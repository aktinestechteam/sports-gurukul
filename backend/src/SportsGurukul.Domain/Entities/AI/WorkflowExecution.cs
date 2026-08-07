using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class WorkflowExecution : BaseEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public AIWorkflowExecutionStatus Status { get; set; } = AIWorkflowExecutionStatus.Pending;
    public AITriggerType TriggerType { get; set; } = AITriggerType.Manual;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public string? ErrorJson { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public int? TotalTokens { get; set; }
    public decimal? TotalCost { get; set; }
    public string? CorrelationId { get; set; }
    public Guid? ExecutedByUserId { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public WorkflowDefinition? WorkflowDefinition { get; set; }
    public ICollection<AgentExecution> AgentExecutions { get; set; } = new List<AgentExecution>();
    public ICollection<ToolExecution> ToolExecutions { get; set; } = new List<ToolExecution>();
}
