using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public class CashfreeGateway : GatewayAdapterBase
{
    public override string ProviderName => "Cashfree";

    private readonly HttpClient _httpClient;
    private readonly GatewayConfig _config;

    public CashfreeGateway(
        HttpClient httpClient,
        GatewayConfig config,
        ILogger<CashfreeGateway> logger,
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
            ? "https://sandbox.cashfree.com/pg"
            : "https://api.cashfree.com/pg";

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("x-api-version", "2025-01-01");
        _httpClient.DefaultRequestHeaders.Add("x-client-id", _config.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("x-client-secret", _config.ApiSecret);
    }

    public override async Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                order_id = request.OrderId,
                order_amount = request.Amount,
                order_currency = request.Currency,
                order_note = request.Description,
                customer_details = new
                {
                    customer_id = request.CustomerId ?? request.OrderId,
                    customer_email = request.CustomerEmail,
                    customer_phone = request.CustomerPhone
                },
                order_meta = new
                {
                    return_url = request.ReturnUrl,
                    notify_url = request.WebhookUrl
                },
                order_tags = request.Notes
            };

            var response = await _httpClient.PostAsJsonAsync("/orders", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = result.GetProperty("order_id").GetString() ?? string.Empty,
                ProviderOrderId = request.OrderId,
                Amount = result.GetProperty("order_amount").GetDecimal(),
                Currency = result.GetProperty("order_currency").GetString() ?? "INR",
                Status = "created",
                CreatedAt = DateTime.UtcNow,
                PaymentPageUrl = result.TryGetProperty("payment_link", out var link) ? link.GetString() : null,
                PaymentLink = result.TryGetProperty("payment_session_id", out var session)
                    ? $"https://payments.cashfree.com/order?session={session}"
                    : null,
                IdempotencyKey = request.IdempotencyKey,
                GatewayMetadata = new Dictionary<string, string>
                {
                    ["order_token"] = result.TryGetProperty("order_token", out var token) ? token.GetString() ?? string.Empty : string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Cashfree create order failed for OrderId {OrderId}", request.OrderId);
            return CreateErrorResponse(ex.Message, request.OrderId);
        }
    }

    public override async Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/orders/{gatewayOrderId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = gatewayOrderId,
                Status = result.GetProperty("order_status").GetString() ?? "authorized",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Cashfree authorize failed for Order {GatewayOrderId}", gatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new { capture_amount = request.Amount };
            var response = await _httpClient.PostAsJsonAsync($"/orders/{request.GatewayOrderId}/capture", payload, cancellationToken);
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
            Logger.LogError(ex, "Cashfree capture failed for {GatewayOrderId}", request.GatewayOrderId);
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
                GatewayPaymentId = result.TryGetProperty("cf_payment_id", out var pid) ? pid.GetString() ?? string.Empty : string.Empty,
                Amount = result.GetProperty("order_amount").GetDecimal(),
                Currency = result.GetProperty("order_currency").GetString() ?? "INR",
                Status = result.GetProperty("order_status").GetString() ?? "unknown",
                Method = result.TryGetProperty("payment_method", out var pm) ? pm.GetString() : null,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Cashfree status check failed for {GatewayOrderId}", gatewayOrderId);
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
                refund_amount = request.Amount,
                refund_reason = request.Reason,
                refund_id = $"ref_{Guid.NewGuid():N}"
            };

            var response = await _httpClient.PostAsJsonAsync($"/orders/{request.GatewayPaymentId}/refunds", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentRefundResponse
            {
                GatewayRefundId = result.GetProperty("refund_id").GetString() ?? string.Empty,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Currency = request.Currency ?? "INR",
                Status = result.GetProperty("refund_status").GetString() ?? "pending",
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Cashfree refund failed for Payment {GatewayPaymentId}", request.GatewayPaymentId);
            return new PaymentRefundResponse { Status = "failed", GatewayPaymentId = request.GatewayPaymentId };
        }
    }

    public override Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Cashfree cancel order {GatewayOrderId}", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId
        });
    }

    public override Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Cashfree void order {GatewayOrderId}", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId
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
            ErrorMessage = isValid ? null : "Invalid Cashfree webhook signature",
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
            TokenId = $"cf_tok_{Guid.NewGuid():N}",
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = ProviderName,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<bool> DeletePaymentMethodAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Cashfree delete payment method {TokenId}", tokenId);
        return Task.FromResult(true);
    }
}
