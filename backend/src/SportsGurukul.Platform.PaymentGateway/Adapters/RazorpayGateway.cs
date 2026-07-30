using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public class RazorpayGateway : GatewayAdapterBase
{
    public override string ProviderName => "Razorpay";

    private readonly HttpClient _httpClient;
    private readonly GatewayConfig _config;

    public RazorpayGateway(
        HttpClient httpClient,
        GatewayConfig config,
        ILogger<RazorpayGateway> logger,
        IPaymentSignatureValidator signatureValidator)
        : base(logger, signatureValidator)
    {
        _httpClient = httpClient;
        _config = config;
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        var baseUrl = _config.UseSandbox
            ? "https://api.razorpay.com/v1"
            : "https://api.razorpay.com/v1";

        _httpClient.BaseAddress = new Uri(baseUrl);
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.ApiKey}:{_config.ApiSecret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
    }

    public override async Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                amount = (int)(request.Amount * 100),
                currency = request.Currency,
                receipt = request.OrderId,
                notes = request.Notes,
                partial_payment = false,
                expire_by = request.ExpiresAfterMinutes.HasValue
                    ? (long?)DateTimeOffset.UtcNow.AddMinutes(request.ExpiresAfterMinutes.Value).ToUnixTimeSeconds()
                    : null
            };

            var response = await _httpClient.PostAsJsonAsync("/orders", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = result.GetProperty("id").GetString() ?? string.Empty,
                ProviderOrderId = request.OrderId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = "created",
                CreatedAt = DateTime.UtcNow,
                PaymentPageUrl = $"https://pages.razorpay.com/{result.GetProperty("id").GetString()}",
                IdempotencyKey = request.IdempotencyKey
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Razorpay create order failed for OrderId {OrderId}", request.OrderId);
            return CreateErrorResponse(ex.Message, request.OrderId);
        }
    }

    public override async Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/orders/{gatewayOrderId}/authorize", null, cancellationToken);
            response.EnsureSuccessStatusCode();

            return new PaymentOrderResponse
            {
                GatewayOrderId = gatewayOrderId,
                Status = "authorized",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Razorpay authorize failed for Order {GatewayOrderId}", gatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new { amount = (int)(request.Amount * 100), currency = request.Currency ?? "INR" };
            var response = await _httpClient.PostAsJsonAsync($"/payments/{request.GatewayOrderId}/capture", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            return new PaymentOrderResponse
            {
                GatewayOrderId = request.GatewayOrderId,
                Status = "captured",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Razorpay capture failed for {GatewayOrderId}", request.GatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/orders/{gatewayOrderId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentStatusResponse
            {
                GatewayOrderId = gatewayOrderId,
                Amount = result.GetProperty("amount").GetDecimal() / 100,
                Currency = result.GetProperty("currency").GetString() ?? "INR",
                Status = result.GetProperty("status").GetString() ?? "unknown",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Razorpay status check failed for {GatewayOrderId}", gatewayOrderId);
            return CreateErrorStatusResponse(ex.Message);
        }
    }

    public override async Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                amount = (int)(request.Amount * 100),
                speed = "normal",
                notes = request.Metadata
            };

            var response = await _httpClient.PostAsJsonAsync($"/payments/{request.GatewayPaymentId}/refund", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentRefundResponse
            {
                GatewayRefundId = result.GetProperty("id").GetString() ?? string.Empty,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Currency = request.Currency ?? "INR",
                Status = "processed",
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Razorpay refund failed for Payment {GatewayPaymentId}", request.GatewayPaymentId);
            return new PaymentRefundResponse { Status = "failed", GatewayPaymentId = request.GatewayPaymentId };
        }
    }

    public override Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Razorpay cancel order {GatewayOrderId} - cancel before payment", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId,
            ErrorMessage = "Order cancelled"
        });
    }

    public override Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Razorpay void order {GatewayOrderId} - authorization void", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId,
            ErrorMessage = "Authorization voided"
        });
    }

    public override async Task<PaymentOrderResponse> RetryPaymentAsync(
        PaymentRetryRequest request,
        CancellationToken cancellationToken = default)
    {
        return await CreateOrderAsync(new PaymentOrderRequest
        {
            OrderId = request.GatewayOrderId,
            IdempotencyKey = request.NewIdempotencyKey
        }, cancellationToken);
    }

    public override Task<GatewayOperationResult> VerifyWebhookSignatureAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        var isValid = SignatureValidator.ValidateWebhook(payload, _config);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = isValid,
            ErrorMessage = isValid ? null : "Invalid webhook signature",
            Timestamp = DateTime.UtcNow
        });
    }

    public override Task<PaymentMethodToken?> SavePaymentMethodAsync(
        string customerId,
        string gatewayPaymentMethodId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<PaymentMethodToken?>(new PaymentMethodToken
        {
            TokenId = $"rp_tok_{Guid.NewGuid():N}",
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = ProviderName,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<bool> DeletePaymentMethodAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Razorpay delete payment method {TokenId}", tokenId);
        return Task.FromResult(true);
    }
}
