using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class WorkflowExecution : BaseEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public WorkflowExecutionStatus Status { get; set; } = WorkflowExecutionStatus.Pending;
    public string? Input { get; set; }
    public string? Output { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? CurrentStep { get; set; }
    public int? TotalSteps { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
}
