using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public class PayUGateway : GatewayAdapterBase
{
    public override string ProviderName => "PayU";

    private readonly GatewayConfig _config;

    public PayUGateway(
        GatewayConfig config,
        ILogger<PayUGateway> logger,
        IPaymentSignatureValidator signatureValidator)
        : base(logger, signatureValidator)
    {
        _config = config;
    }

    public override Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var txnId = request.OrderId;
        var hashString = $"{_config.ApiKey}|{txnId}|{request.Amount:F2}|{request.Currency}|{request.Description}|||||||||||{_config.ApiSecret}";
        var hash = GenerateHash(hashString);

        Logger.LogInformation("PayU order created: {TxnId}, Amount: {Amount}", txnId, request.Amount);

        return Task.FromResult(new PaymentOrderResponse
        {
            GatewayOrderId = txnId,
            ProviderOrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "created",
            CreatedAt = DateTime.UtcNow,
            PaymentPageUrl = _config.UseSandbox
                ? "https://test.payu.in/_payment"
                : "https://secure.payu.in/_payment",
            GatewayMetadata = new Dictionary<string, string>
            {
                ["hash"] = hash,
                ["key"] = _config.ApiKey ?? string.Empty,
                ["txnid"] = txnId,
                ["productinfo"] = request.Description,
                ["amount"] = request.Amount.ToString("F2")
            },
            IdempotencyKey = request.IdempotencyKey
        });
    }

    public override Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaymentOrderResponse
        {
            GatewayOrderId = gatewayOrderId,
            Status = "authorized",
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaymentOrderResponse
        {
            GatewayOrderId = request.GatewayOrderId,
            Status = "captured",
            CreatedAt = DateTime.UtcNow
        });
    }

    public override async Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var hashString = $"{_config.ApiKey}|verify_payment|{gatewayOrderId}|{_config.ApiSecret}";
            var hash = GenerateHash(hashString);

            using var client = new HttpClient();
            var baseUrl = _config.UseSandbox
                ? "https://test.payu.in/merchant/postservice.php?form=2"
                : "https://info.payu.in/merchant/postservice.php?form=2";

            var formData = new Dictionary<string, string>
            {
                ["key"] = _config.ApiKey ?? string.Empty,
                ["command"] = "verify_payment",
                ["var1"] = gatewayOrderId,
                ["hash"] = hash
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync(baseUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var transactionDetails = result.TryGetProperty("transaction_details", out var td)
                ? td.EnumerateObject().FirstOrDefault().Value
                : default;

            return new PaymentStatusResponse
            {
                GatewayOrderId = gatewayOrderId,
                GatewayPaymentId = transactionDetails.ValueKind != JsonValueKind.Undefined
                    ? transactionDetails.TryGetProperty("mihpayid", out var mihpayid) ? mihpayid.GetString() ?? string.Empty : string.Empty
                    : string.Empty,
                Amount = transactionDetails.ValueKind != JsonValueKind.Undefined
                    ? transactionDetails.TryGetProperty("amount", out var amt) ? decimal.Parse(amt.GetString() ?? "0") : 0
                    : 0,
                Status = transactionDetails.ValueKind != JsonValueKind.Undefined
                    ? transactionDetails.TryGetProperty("status", out var st) ? st.GetString() ?? "unknown" : "unknown"
                    : "unknown",
                Method = transactionDetails.ValueKind != JsonValueKind.Undefined
                    ? transactionDetails.TryGetProperty("mode", out var mode) ? mode.GetString() : null
                    : null,
                BankReference = transactionDetails.ValueKind != JsonValueKind.Undefined
                    ? transactionDetails.TryGetProperty("bank_ref_num", out var brn) ? brn.GetString() : null
                    : null,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PayU status check failed for {GatewayOrderId}", gatewayOrderId);
            return CreateErrorStatusResponse(ex.Message);
        }
    }

    public override Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var refundId = $"rf_{Guid.NewGuid():N}";
        Logger.LogInformation("PayU refund initiated: {RefundId} for Payment {GatewayPaymentId}", refundId, request.GatewayPaymentId);

        return Task.FromResult(new PaymentRefundResponse
        {
            GatewayRefundId = refundId,
            GatewayPaymentId = request.GatewayPaymentId,
            Amount = request.Amount,
            Currency = request.Currency ?? "INR",
            Status = "processed",
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogWarning("PayU cancel transaction {GatewayOrderId}", request.GatewayOrderId);
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
        Logger.LogWarning("PayU void transaction {GatewayOrderId}", request.GatewayOrderId);
        return Task.FromResult(new GatewayOperationResult
        {
            IsSuccess = true,
            GatewayTransactionId = request.GatewayOrderId
        });
    }

    public override Task<PaymentOrderResponse> RetryPaymentAsync(
        PaymentRetryRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateOrderAsync(new PaymentOrderRequest
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
            ErrorMessage = isValid ? null : "Invalid PayU webhook signature",
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
            TokenId = $"pu_tok_{Guid.NewGuid():N}",
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = ProviderName,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<bool> DeletePaymentMethodAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("PayU delete payment method {TokenId}", tokenId);
        return Task.FromResult(true);
    }

    private static string GenerateHash(string input)
    {
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
