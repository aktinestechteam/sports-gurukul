namespace SportsGurukul.Platform.AI.Models;

public enum ApprovalType
{
    ToolCall,
    WorkflowStep,
    AgentOutput,
    ResourceAction
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    TimedOut,
    Escalated,
    Cancelled
}

public enum ApprovalPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public class CreateApprovalRequest
{
    public ApprovalType Type { get; set; } = ApprovalType.ToolCall;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Action { get; set; }
    public object? Payload { get; set; }
    public string? RequestedBy { get; set; }
    public string? ApproverId { get; set; }
    public string? RequiredRole { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public string? RunId { get; set; }
    public TimeSpan? ExpiresIn { get; set; }
    public TimeSpan? EscalationThreshold { get; set; }
    public string? EscalationTarget { get; set; }
    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;
}

public class ApprovalRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApprovalType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Action { get; set; }
    public object? Payload { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;
    public string? RequestedBy { get; set; }
    public string? ApproverId { get; set; }
    public string? RequiredRole { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public string? RunId { get; set; }
    public string? DecisionReason { get; set; }
    public int EscalationLevel { get; set; }
    public string? EscalationTarget { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class ApprovalDecision
{
    public Guid RequestId { get; set; }
    public bool Approved { get; set; }
    public string? DecidedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
}

public class ApprovalSummary
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int TimedOut { get; set; }
    public int Escalated { get; set; }
}
