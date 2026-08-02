namespace SportsGurukul.Platform.AI.Models;

public enum WorkflowStatus
{
    Draft,
    Active,
    Running,
    WaitingForApproval,
    Paused,
    Completed,
    Failed,
    Cancelled
}

public enum WorkflowStepStatus
{
    Pending,
    Ready,
    Running,
    Succeeded,
    Failed,
    Skipped,
    WaitingForApproval,
    Cancelled,
    Compensating,
    Compensated
}

public enum WorkflowStepType
{
    Task,
    Approval,
    Parallel,
    Condition,
    Wait
}

public enum RetryPolicy
{
    None,
    Fixed,
    Linear,
    Exponential
}

public class WorkflowDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; } = 1;
    public List<WorkflowStepDefinition> Steps { get; set; } = [];
    public Dictionary<string, object?>? Metadata { get; set; }
}

public class WorkflowStepDefinition
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public WorkflowStepType Type { get; set; } = WorkflowStepType.Task;
    public string? ToolName { get; set; }
    public Dictionary<string, object?> ToolArguments { get; set; } = new();
    public List<string> DependsOn { get; set; } = [];
    public string? Condition { get; set; }
    public int RetryMax { get; set; }
    public RetryPolicy RetryPolicy { get; set; } = RetryPolicy.None;
    public int RetryDelaySeconds { get; set; } = 1;
    public string? CompensatingStepId { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovalRequiredRole { get; set; }
    public int? TimeoutSeconds { get; set; }
    public List<string> BranchStepIds { get; set; } = [];
}

public class WorkflowStartOptions
{
    public string? TenantId { get; set; }
    public string? CreatedBy { get; set; }
    public string? CorrelationId { get; set; }
    public Dictionary<string, object?>? Input { get; set; }
    public Dictionary<string, object?>? InitialState { get; set; }
    public bool RunStepsInParallel { get; set; }
}

public class WorkflowExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DefinitionName { get; set; } = string.Empty;
    public int DefinitionVersion { get; set; } = 1;
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Running;
    public Dictionary<string, object?> State { get; set; } = new();
    public List<WorkflowStepExecution> Steps { get; set; } = [];
    public WorkflowDefinition? Definition { get; set; }
    public string? TenantId { get; set; }
    public string? CreatedBy { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public int Revision { get; set; }
}

public class WorkflowStepExecution
{
    public string StepId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.Pending;
    public int Attempts { get; set; }
    public object? Input { get; set; }
    public object? Output { get; set; }
    public string? Error { get; set; }
    public string? ApprovalRequestId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public class WorkflowCheckpoint
{
    public Guid ExecutionId { get; set; }
    public Dictionary<string, object?> State { get; set; } = new();
    public List<WorkflowStepExecution> Steps { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
