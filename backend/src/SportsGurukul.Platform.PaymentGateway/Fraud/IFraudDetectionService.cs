namespace SportsGurukul.Platform.PaymentGateway.Fraud;

public interface IFraudDetectionService
{
    Task<FraudAssessmentResult> AssessAsync(
        FraudAssessmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> IsSuspiciousAsync(
        string customerId,
        decimal amount,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsBlacklistedAsync(
        string customerId,
        CancellationToken cancellationToken = default);

    Task<bool> IsHighRiskTransactionAsync(
        decimal amount,
        string paymentMethod,
        string? customerId = null,
        CancellationToken cancellationToken = default);
}

public class FraudAssessmentRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string PaymentMethod { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceFingerprint { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string? OrderId { get; set; }
}

public class FraudAssessmentResult
{
    public bool IsFraudulent { get; set; }
    public decimal RiskScore { get; set; }
    public string RiskLevel { get; set; } = "low";
    public List<string> Flags { get; set; } = [];
    public string? RecommendedAction { get; set; }
    public string? AssessmentId { get; set; }
}
