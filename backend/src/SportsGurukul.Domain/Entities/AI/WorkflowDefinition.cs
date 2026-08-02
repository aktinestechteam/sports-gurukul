using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class WorkflowDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;
    public string? Steps { get; set; }
    public string? Triggers { get; set; }
    public string? Conditions { get; set; }
    public string? Variables { get; set; }
    public int Version { get; set; } = 1;
    public byte[] RowVersion { get; set; } = [];

    public ICollection<WorkflowExecution>? Executions { get; set; }
}
