namespace SportsGurukul.Platform.AI.Models;

public enum SecurityRiskLevel
{
    Safe,
    Suspicious,
    High,
    Blocked
}

public class TenantContext
{
    public string? TenantId { get; set; }
    public string? UserId { get; set; }
    public string? CorrelationId { get; set; }
}

public class PromptInjectionAssessment
{
    public SecurityRiskLevel RiskLevel { get; set; } = SecurityRiskLevel.Safe;
    public bool IsFlagged => RiskLevel >= SecurityRiskLevel.Suspicious;
    public IReadOnlyList<string> Indicators { get; set; } = [];
    public string? SanitizedInput { get; set; }
}

public class OutputValidationResult
{
    public bool IsValid { get; set; } = true;
    public IReadOnlyList<string> Violations { get; set; } = [];
    public string? SanitizedOutput { get; set; }
}

public class AuditRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = string.Empty;
    public string? Actor { get; set; }
    public string? TenantId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string Severity { get; set; } = "Info";
    public string? Details { get; set; }
    public string? CorrelationId { get; set; }
}

public class AuditQuery
{
    public string? TenantId { get; set; }
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Limit { get; set; } = 100;
}
