namespace SportsGurukul.Platform.AI.Models;

public enum TaskPriority
{
    Critical,
    High,
    Medium,
    Low
}

public enum TaskState
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Blocked,
    Skipped
}

public class PlanningGoal
{
    public string Description { get; set; } = string.Empty;
    public string? Input { get; set; }
    public IReadOnlyList<string>? AcceptanceCriteria { get; set; }
    public string? SessionId { get; set; }
    public IDictionary<string, object?>? Metadata { get; set; }
}

public class Plan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Goal { get; set; } = string.Empty;
    public IReadOnlyList<PlanStep> Steps { get; set; } = [];
    public double Confidence { get; set; } = 1.0;
    public int Revision { get; set; } = 1;
    public string? ReplanReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PlanStep
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ToolName { get; set; }
    public IDictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public IReadOnlyList<string> DependsOn { get; set; } = [];
    public TaskState State { get; set; } = TaskState.Pending;
    public bool RequiresApproval { get; set; }
    public string? Result { get; set; }
}

public class ReflectionRequest
{
    public string? PlanId { get; set; }
    public string? Goal { get; set; }
    public IReadOnlyList<AgentTaskResult>? CompletedSteps { get; set; }
    public PlanStep? CurrentStep { get; set; }
    public string? Insight { get; set; }
    public IDictionary<string, object?>? Context { get; set; }
}

public class Reflection
{
    public string? PlanId { get; set; }
    public double Score { get; set; }
    public string Insight { get; set; } = string.Empty;
    public string? Improvement { get; set; }
    public bool ShouldReplan { get; set; }
    public bool ShouldStop { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SelfEvaluationRequest
{
    public string? RunId { get; set; }
    public string? Goal { get; set; }
    public IReadOnlyList<AgentTaskResult>? Tasks { get; set; }
    public string? FinalAnswer { get; set; }
    public IDictionary<string, object?>? Context { get; set; }
}

public class SelfEvaluation
{
    public string? RunId { get; set; }
    public double Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public IReadOnlyList<string> Strengths { get; set; } = [];
    public IReadOnlyList<string> Weaknesses { get; set; } = [];
    public IReadOnlyList<string> Improvements { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
