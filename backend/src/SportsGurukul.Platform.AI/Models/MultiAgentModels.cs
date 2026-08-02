namespace SportsGurukul.Platform.AI.Models;

public enum AggregationStrategy
{
    FirstSuccess,
    BestScore,
    Concatenate,
    Merge,
    Vote
}

public class SupervisorRunRequest
{
    public string Goal { get; set; } = string.Empty;
    public string? Input { get; set; }
    public IReadOnlyList<string>? WorkerAgentIds { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public AggregationStrategy Strategy { get; set; } = AggregationStrategy.FirstSuccess;
    public bool DelegateAllSteps { get; set; } = true;
}

public class SupervisorRunResult
{
    public Guid RunId { get; set; } = Guid.NewGuid();
    public string Goal { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string? Answer { get; set; }
    public IReadOnlyList<DelegatedTaskResult> Results { get; set; } = [];
    public AggregationStrategy Strategy { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}

public class DelegatedTask
{
    public Guid TaskId { get; set; } = Guid.NewGuid();
    public string Goal { get; set; } = string.Empty;
    public string? Input { get; set; }
    public string? AssignedAgentId { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}

public class DelegatedTaskResult
{
    public Guid TaskId { get; set; }
    public string Goal { get; set; } = string.Empty;
    public string? AgentId { get; set; }
    public bool Succeeded { get; set; }
    public string? Answer { get; set; }
    public string? Error { get; set; }
    public double? Score { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}

public class AgentRoutingDecision
{
    public Guid TaskId { get; set; }
    public string? SelectedAgentId { get; set; }
    public string? Reason { get; set; }
    public double Confidence { get; set; }
}

public class AggregationResult
{
    public bool Succeeded { get; set; }
    public string? Answer { get; set; }
    public AggregationStrategy Strategy { get; set; }
    public int ResultCount { get; set; }
    public IReadOnlyList<string>? Notes { get; set; }
}
