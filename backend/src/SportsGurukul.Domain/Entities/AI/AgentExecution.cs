using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AgentExecution : BaseEntity
{
    public Guid AgentDefinitionId { get; set; }
    public Guid? WorkflowExecutionId { get; set; }
    public AIAgentExecutionStatus Status { get; set; } = AIAgentExecutionStatus.Pending;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public string? ErrorJson { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? Cost { get; set; }
    public Guid? ExecutedByUserId { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AgentDefinition? AgentDefinition { get; set; }
    public WorkflowExecution? WorkflowExecution { get; set; }
    public ICollection<ToolExecution> ToolExecutions { get; set; } = new List<ToolExecution>();
}
