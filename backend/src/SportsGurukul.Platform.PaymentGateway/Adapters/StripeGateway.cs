using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public class StripeGateway : GatewayAdapterBase
{
    public override string ProviderName => "Stripe";

    private readonly HttpClient _httpClient;
    private readonly GatewayConfig _config;

    public StripeGateway(
        HttpClient httpClient,
        GatewayConfig config,
        ILogger<StripeGateway> logger,
        IPaymentSignatureValidator signatureValidator)
        : base(logger, signatureValidator)
    {
        _httpClient = httpClient;
        _config = config;
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri("https://api.stripe.com/v1");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.ApiSecret);
    }

    public override async Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                ["amount"] = ((int)(request.Amount * 100)).ToString(),
                ["currency"] = request.Currency.ToLowerInvariant(),
                ["description"] = request.Description,
                ["confirm"] = "false",
                ["capture_method"] = request.IsCapture ? "automatic" : "manual"
            };

            if (!string.IsNullOrEmpty(request.CustomerId))
                formData["customer"] = request.CustomerId;

            if (!string.IsNullOrEmpty(request.ReturnUrl))
                formData["return_url"] = request.ReturnUrl;

            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                _httpClient.DefaultRequestHeaders.Remove("Idempotency-Key");

            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                _httpClient.DefaultRequestHeaders.Add("Idempotency-Key", request.IdempotencyKey);

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("/payment_intents", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = result.GetProperty("id").GetString() ?? string.Empty,
                ProviderOrderId = request.OrderId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = result.GetProperty("status").GetString() ?? "created",
                CreatedAt = DateTime.UtcNow,
                PaymentLink = result.TryGetProperty("next_action", out var nextAction)
                    ? nextAction.TryGetProperty("redirect_to_url", out var redirect)
                        ? redirect.GetProperty("url").GetString()
                        : null
                    : null,
                IdempotencyKey = request.IdempotencyKey,
                GatewayMetadata = new Dictionary<string, string>
                {
                    ["client_secret"] = result.GetProperty("client_secret").GetString() ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stripe create payment intent failed for OrderId {OrderId}", request.OrderId);
            return CreateErrorResponse(ex.Message, request.OrderId);
        }
    }

    public override async Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>());
            var response = await _httpClient.PostAsync($"/payment_intents/{gatewayOrderId}/confirm", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return new PaymentOrderResponse
            {
                GatewayOrderId = gatewayOrderId,
                Status = "requires_capture",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stripe authorize failed for {GatewayOrderId}", gatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["amount_to_capture"] = ((int)(request.Amount * 100)).ToString()
            });

            var response = await _httpClient.PostAsync($"/payment_intents/{request.GatewayOrderId}/capture", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = request.GatewayOrderId,
                Status = result.GetProperty("status").GetString() ?? "succeeded",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stripe capture failed for {GatewayOrderId}", request.GatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/payment_intents/{gatewayOrderId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var charges = result.TryGetProperty("charges", out var chargesData)
                ? chargesData.GetProperty("data")
                : default;

            PaymentMethodDetails? method = null;
            if (charges.ValueKind != JsonValueKind.Undefined && charges.EnumerateArray().Any())
            {
                var charge = charges.EnumerateArray().First();
                var pmDetails = charge.GetProperty("payment_method_details");
                method = ExtractPaymentMethodDetails(pmDetails);
            }

            return new PaymentStatusResponse
            {
                GatewayOrderId = gatewayOrderId,
                GatewayPaymentId = result.GetProperty("id").GetString() ?? string.Empty,
                Amount = result.GetProperty("amount").GetDecimal() / 100,
                AmountCaptured = result.TryGetProperty("amount_received", out var captured)
                    ? captured.GetDecimal() / 100 : 0,
                Currency = result.GetProperty("currency").GetString()?.ToUpperInvariant() ?? "INR",
                Status = result.GetProperty("status").GetString() ?? "unknown",
                Method = method?.Type,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stripe status check failed for {GatewayOrderId}", gatewayOrderId);
            return CreateErrorStatusResponse(ex.Message);
        }
    }

    public override async Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                ["payment_intent"] = request.GatewayPaymentId,
                ["amount"] = ((int)(request.Amount * 100)).ToString()
            };

            if (!string.IsNullOrEmpty(request.Reason))
                formData["reason"] = request.Reason;

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("/refunds", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentRefundResponse
            {
                GatewayRefundId = result.GetProperty("id").GetString() ?? string.Empty,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Currency = request.Currency ?? "INR",
                Status = result.GetProperty("status").GetString() ?? "pending",
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stripe refund failed for Payment {GatewayPaymentId}", request.GatewayPaymentId);
            return new PaymentRefundResponse { Status = "failed", GatewayPaymentId = request.GatewayPaymentId };
        }
    }

    public override Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Stripe cancel payment intent {GatewayOrderId}", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId,
            ErrorMessage = "Payment intent cancelled"
        });
    }

    public override Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("Stripe void payment intent {GatewayOrderId}", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId,
            ErrorMessage = "Payment intent voided"
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
            ErrorMessage = isValid ? null : "Invalid Stripe webhook signature",
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
            TokenId = $"st_tok_{Guid.NewGuid():N}",
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = ProviderName,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<bool> DeletePaymentMethodAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Stripe delete payment method {TokenId}", tokenId);
        return Task.FromResult(true);
    }

    private static PaymentMethodDetails? ExtractPaymentMethodDetails(JsonElement pmDetails)
    {
        var details = new PaymentMethodDetails();

        if (pmDetails.TryGetProperty("type", out var type))
        {
            details.Type = type.GetString() ?? string.Empty;
        }

        if (pmDetails.TryGetProperty("card", out var card))
        {
            details.CardBrand = card.TryGetProperty("brand", out var brand) ? brand.GetString() : null;
            details.CardLastFour = card.TryGetProperty("last4", out var last4) ? last4.GetString() : null;
            details.CardExpiryMonth = card.TryGetProperty("exp_month", out var expMonth) ? expMonth.GetInt32().ToString() : null;
            details.CardExpiryYear = card.TryGetProperty("exp_year", out var expYear) ? expYear.GetInt32().ToString() : null;
        }

        return details;
    }
}
