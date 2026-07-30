namespace SportsGurukul.Platform.PaymentGateway.Models;

public class PaymentOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Description { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string? ReturnUrl { get; set; }
    public string? WebhookUrl { get; set; }
    public int? ExpiresAfterMinutes { get; set; }
    public string? IdempotencyKey { get; set; }
    public bool IsCapture { get; set; } = true;
    public Dictionary<string, string>? Notes { get; set; }
}

public class PaymentOrderResponse
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public string ProviderOrderId { get; set; } = string.Empty;
    public string? PaymentLink { get; set; }
    public string? PaymentPageUrl { get; set; }
    public string? QrCode { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = "created";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string>? GatewayMetadata { get; set; }
    public string? IdempotencyKey { get; set; }
}

public class PaymentCaptureRequest
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
}

public class PaymentRefundRequest
{
    public string GatewayPaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Reason { get; set; }
    public string? IdempotencyKey { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class PaymentRefundResponse
{
    public string GatewayRefundId { get; set; } = string.Empty;
    public string GatewayPaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = "pending";
    public string? Reason { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PaymentStatusResponse
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public string GatewayPaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountCaptured { get; set; }
    public decimal AmountRefunded { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = string.Empty;
    public string? Method { get; set; }
    public string? BankReference { get; set; }
    public string? CardId { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardNetwork { get; set; }
    public string? UpiVpa { get; set; }
    public DateTime? CapturedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, string>? GatewayMetadata { get; set; }
    public List<PaymentRefundResponse>? Refunds { get; set; }
}

public class PaymentVoidRequest
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class PaymentCancelRequest
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class PaymentRetryRequest
{
    public string GatewayOrderId { get; set; } = string.Empty;
    public string? NewIdempotencyKey { get; set; }
}

public class GatewayConfig
{
    public string Provider { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? MerchantId { get; set; }
    public string? WebhookSecret { get; set; }
    public string? BaseUrl { get; set; }
    public bool UseSandbox { get; set; } = true;
    public Dictionary<string, string>? AdditionalSettings { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public Dictionary<string, string>? WebhookConfig { get; set; }
}

public class PaymentMethodDetails
{
    public string Type { get; set; } = string.Empty;
    public string? CardBrand { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardExpiryMonth { get; set; }
    public string? CardExpiryYear { get; set; }
    public string? CardHolderName { get; set; }
    public string? UpiVpa { get; set; }
    public string? BankName { get; set; }
    public string? WalletType { get; set; }
    public string? NetBankingBank { get; set; }
}

public class PaymentMethodToken
{
    public string TokenId { get; set; } = string.Empty;
    public string GatewayTokenId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? MaskedCardNumber { get; set; }
    public string? CardBrand { get; set; }
    public string? CardExpiryMonth { get; set; }
    public string? CardExpiryYear { get; set; }
    public string? CardHolderName { get; set; }
    public string? CustomerId { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}

public class GatewayOperationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? GatewayTransactionId { get; set; }
    public Dictionary<string, string>? GatewayResponse { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
