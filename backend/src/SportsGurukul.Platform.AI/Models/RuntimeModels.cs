namespace SportsGurukul.Platform.AI.Models;

public enum AgentState
{
    Idle,
    Planning,
    Executing,
    WaitingForApproval,
    Paused,
    Completed,
    Failed,
    Cancelled
}

public enum AgentRole
{
    Worker,
    Supervisor,
    Specialist
}

public class AgentDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AgentRole Role { get; set; } = AgentRole.Worker;
    public string? SystemPrompt { get; set; }
    public string Provider { get; set; } = "stub";
    public string Model { get; set; } = "default";
    public int MaxIterations { get; set; } = 10;
    public int MaxToolCalls { get; set; } = 50;
    public bool RequiresApproval { get; set; }
    public string? ApprovalPolicy { get; set; }
    public IReadOnlyList<string> AllowedToolNames { get; set; } = [];
    public IReadOnlyList<string> Capabilities { get; set; } = [];
    public bool EnableReflection { get; set; } = true;
    public bool EnableSelfEvaluation { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AgentContext
{
    public Guid RunId { get; set; }
    public string AgentId { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public string? UserId { get; set; }
    public string? CorrelationId { get; set; }
    public AgentDefinition Definition { get; set; } = new();
}

public class AgentRunRequest
{
    public string AgentId { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string? Input { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public string? UserId { get; set; }
    public string? CorrelationId { get; set; }
    public AgentRunOptions? Options { get; set; }
}

public class AgentRunOptions
{
    public int? MaxIterations { get; set; }
    public int? MaxToolCalls { get; set; }
    public bool? EnableReflection { get; set; }
    public bool? EnableSelfEvaluation { get; set; }
    public TimeSpan? Timeout { get; set; }
}

public class AgentRunResult
{
    public Guid RunId { get; set; }
    public string AgentId { get; set; } = string.Empty;
    public AgentState Status { get; set; }
    public string? Answer { get; set; }
    public IReadOnlyList<AgentTaskResult> Tasks { get; set; } = [];
    public ModelUsage? Usage { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Error { get; set; }
    public int IterationCount { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class AgentTaskResult
{
    public PlanStep Step { get; set; } = new();
    public ToolResult? ToolResult { get; set; }
    public string? ModelOutput { get; set; }
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
}

public class AgentSession
{
    public Guid RunId { get; set; }
    public string AgentId { get; set; } = string.Empty;
    public AgentState State { get; set; } = AgentState.Idle;
    public string? Goal { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public int IterationCount { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public string? LastError { get; set; }
}
