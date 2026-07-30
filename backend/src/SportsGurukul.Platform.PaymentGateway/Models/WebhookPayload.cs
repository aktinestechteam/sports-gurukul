namespace SportsGurukul.Platform.PaymentGateway.Models;

public class WebhookPayload
{
    public string RawBody { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string GatewayPaymentId { get; set; } = string.Empty;
    public string GatewayOrderId { get; set; } = string.Empty;
    public string? GatewayRefundId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? Status { get; set; }
    public string? FailureReason { get; set; }
    public string? BankReference { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardNetwork { get; set; }
    public string? UpiVpa { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? WebhookId { get; set; }
    public DateTime? WebhookTimestamp { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, object>? RawEvent { get; set; }
    public string? DisputeId { get; set; }
    public string? DisputeReason { get; set; }
    public string? DisputeStatus { get; set; }
    public DateTime? DisputeDate { get; set; }
}

public enum WebhookEventType
{
    PaymentSuccess,
    PaymentFailed,
    PaymentCaptured,
    PaymentAuthorized,
    RefundCompleted,
    RefundFailed,
    DisputeCreated,
    DisputeResolved,
    Chargeback,
    OrderCreated,
    OrderExpired,
    Unknown
}

public class WebhookResult
{
    public bool IsProcessed { get; set; }
    public bool IsIdempotent { get; set; }
    public WebhookEventType EventType { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}
