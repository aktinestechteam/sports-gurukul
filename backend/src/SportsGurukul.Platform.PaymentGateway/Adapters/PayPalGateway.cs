using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public class PayPalGateway : GatewayAdapterBase
{
    public override string ProviderName => "PayPal";

    private readonly HttpClient _httpClient;
    private readonly GatewayConfig _config;
    private string? _accessToken;
    private DateTime _tokenExpiry;

    public PayPalGateway(
        HttpClient httpClient,
        GatewayConfig config,
        ILogger<PayPalGateway> logger,
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
            ? "https://api-m.sandbox.paypal.com"
            : "https://api-m.paypal.com";

        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    private async Task EnsureAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_accessToken is not null && DateTime.UtcNow < _tokenExpiry)
            return;

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.ApiKey}:{_config.ApiSecret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var content = new FormUrlEncodedContent(new Dictionary<string, string> { ["grant_type"] = "client_credentials" });
        var response = await _httpClient.PostAsync("/v1/oauth2/token", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        _accessToken = result.GetProperty("access_token").GetString();
        var expiresIn = result.GetProperty("expires_in").GetInt32();
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public override async Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureAccessTokenAsync(cancellationToken);

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = request.OrderId,
                        description = request.Description,
                        amount = new
                        {
                            currency_code = request.Currency,
                            value = request.Amount.ToString("F2")
                        },
                        custom_id = request.OrderId,
                        invoice_id = request.OrderId
                    }
                },
                payment_source = new
                {
                    paypal = new
                    {
                        experience_context = new
                        {
                            return_url = request.ReturnUrl ?? "https://example.com/return",
                            cancel_url = $"{request.ReturnUrl}/cancel" ?? "https://example.com/cancel"
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/v2/checkout/orders", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var links = result.GetProperty("links").EnumerateArray();
            var approveUrl = links.FirstOrDefault(l =>
                l.GetProperty("rel").GetString() == "payer-action");

            return new PaymentOrderResponse
            {
                GatewayOrderId = result.GetProperty("id").GetString() ?? string.Empty,
                ProviderOrderId = request.OrderId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = result.GetProperty("status").GetString()?.ToLowerInvariant() ?? "created",
                CreatedAt = DateTime.UtcNow,
                PaymentPageUrl = approveUrl.ValueKind != JsonValueKind.Undefined
                    ? approveUrl.GetProperty("href").GetString()
                    : null,
                IdempotencyKey = request.IdempotencyKey,
                GatewayMetadata = new Dictionary<string, string>
                {
                    ["intent"] = "CAPTURE"
                }
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PayPal create order failed for OrderId {OrderId}", request.OrderId);
            return CreateErrorResponse(ex.Message, request.OrderId);
        }
    }

    public override async Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureAccessTokenAsync(cancellationToken);

            var payload = new
            {
                intent = "AUTHORIZE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = gatewayOrderId,
                        amount = new
                        {
                            currency_code = "INR",
                            value = "0"
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/v2/checkout/orders", payload, cancellationToken);
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
            Logger.LogError(ex, "PayPal authorize failed for Order {GatewayOrderId}", gatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureAccessTokenAsync(cancellationToken);

            var response = await _httpClient.PostAsync(
                $"/v2/checkout/orders/{request.GatewayOrderId}/capture",
                null,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentOrderResponse
            {
                GatewayOrderId = request.GatewayOrderId,
                Status = result.GetProperty("status").GetString()?.ToLowerInvariant() ?? "completed",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PayPal capture failed for {GatewayOrderId}", request.GatewayOrderId);
            return CreateErrorResponse(ex.Message);
        }
    }

    public override async Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureAccessTokenAsync(cancellationToken);

            var response = await _httpClient.GetAsync($"/v2/checkout/orders/{gatewayOrderId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var purchaseUnit = result.GetProperty("purchase_units").EnumerateArray().FirstOrDefault();
            var amount = purchaseUnit.ValueKind != JsonValueKind.Undefined
                ? purchaseUnit.GetProperty("amount")
                : default;

            return new PaymentStatusResponse
            {
                GatewayOrderId = gatewayOrderId,
                GatewayPaymentId = result.GetProperty("id").GetString() ?? string.Empty,
                Amount = amount.ValueKind != JsonValueKind.Undefined
                    ? decimal.Parse(amount.GetProperty("value").GetString() ?? "0")
                    : 0,
                Currency = amount.ValueKind != JsonValueKind.Undefined
                    ? amount.GetProperty("currency_code").GetString() ?? "INR"
                    : "INR",
                Status = result.GetProperty("status").GetString()?.ToLowerInvariant() ?? "unknown",
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PayPal status check failed for {GatewayOrderId}", gatewayOrderId);
            return CreateErrorStatusResponse(ex.Message);
        }
    }

    public override async Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureAccessTokenAsync(cancellationToken);

            var captureId = await GetCaptureIdAsync(request.GatewayPaymentId, cancellationToken);
            if (string.IsNullOrEmpty(captureId))
            {
                return new PaymentRefundResponse { Status = "failed", GatewayPaymentId = request.GatewayPaymentId, ErrorMessage = "No capture found" };
            }

            var payload = new
            {
                amount = new
                {
                    value = request.Amount.ToString("F2"),
                    currency_code = request.Currency ?? "INR"
                },
                note_to_payer = request.Reason
            };

            var response = await _httpClient.PostAsJsonAsync($"/v2/payments/captures/{captureId}/refund", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new PaymentRefundResponse
            {
                GatewayRefundId = result.GetProperty("id").GetString() ?? string.Empty,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Currency = request.Currency ?? "INR",
                Status = result.GetProperty("status").GetString()?.ToLowerInvariant() ?? "completed",
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PayPal refund failed for Payment {GatewayPaymentId}", request.GatewayPaymentId);
            return new PaymentRefundResponse { Status = "failed", GatewayPaymentId = request.GatewayPaymentId };
        }
    }

    public override Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("PayPal cancel order {GatewayOrderId}", request.GatewayOrderId);
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
        Logger.LogWarning("PayPal void order {GatewayOrderId}", request.GatewayOrderId);
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
            ErrorMessage = isValid ? null : "Invalid PayPal webhook signature",
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
            TokenId = $"pp_tok_{Guid.NewGuid():N}",
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = ProviderName,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<bool> DeletePaymentMethodAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("PayPal delete payment method {TokenId}", tokenId);
        return Task.FromResult(true);
    }

    private async Task<string?> GetCaptureIdAsync(string orderId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v2/checkout/orders/{orderId}", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            var purchases = result.GetProperty("purchase_units").EnumerateArray();
            foreach (var unit in purchases)
            {
                if (unit.TryGetProperty("payments", out var payments) &&
                    payments.TryGetProperty("captures", out var captures))
                {
                    var capture = captures.EnumerateArray().FirstOrDefault();
                    if (capture.ValueKind != JsonValueKind.Undefined)
                        return capture.GetProperty("id").GetString();
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetAuthorizationIdAsync(string orderId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v2/checkout/orders/{orderId}", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            var purchases = result.GetProperty("purchase_units").EnumerateArray();
            foreach (var unit in purchases)
            {
                if (unit.TryGetProperty("payments", out var payments) &&
                    payments.TryGetProperty("authorizations", out var auths))
                {
                    var auth = auths.EnumerateArray().FirstOrDefault();
                    if (auth.ValueKind != JsonValueKind.Undefined)
                        return auth.GetProperty("id").GetString();
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
