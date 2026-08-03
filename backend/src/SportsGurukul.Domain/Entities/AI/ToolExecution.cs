using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ToolExecution : BaseEntity
{
    public Guid ToolDefinitionId { get; set; }
    public Guid? AgentExecutionId { get; set; }
    public Guid? WorkflowExecutionId { get; set; }
    public AIToolExecutionStatus Status { get; set; } = AIToolExecutionStatus.Pending;
    public string? RequestJson { get; set; }
    public string? ResponseJson { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public int? TokenCount { get; set; }
    public decimal? Cost { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ToolDefinition? ToolDefinition { get; set; }
    public AgentExecution? AgentExecution { get; set; }
    public WorkflowExecution? WorkflowExecution { get; set; }
}
