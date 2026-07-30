namespace SportsGurukul.Platform.PaymentGateway.Fraud;

public interface IRiskAssessmentService
{
    Task<RiskScore> CalculateRiskScoreAsync(
        RiskAssessmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RequiresAdditionalVerificationAsync(
        string customerId,
        decimal amount,
        CancellationToken cancellationToken = default);

    Task<RiskLevel> DetermineRiskLevelAsync(
        decimal riskScore,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRiskFlagsAsync(
        string customerId,
        CancellationToken cancellationToken = default);
}

public class RiskAssessmentRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string PaymentMethod { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? DeviceId { get; set; }
    public int TransactionCountLast24Hours { get; set; }
    public decimal TotalAmountLast24Hours { get; set; }
    public bool IsNewCustomer { get; set; }
    public bool IsHighValueTransaction { get; set; }
    public string? CountryCode { get; set; }
}

public class RiskScore
{
    public decimal Score { get; set; }
    public RiskLevel Level { get; set; } = RiskLevel.Low;
    public List<string> Factors { get; set; } = [];
}

public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}
