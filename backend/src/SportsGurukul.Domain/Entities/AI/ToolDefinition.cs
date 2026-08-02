using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ToolDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ToolType Type { get; set; }
    public ToolStatus Status { get; set; } = ToolStatus.Active;
    public string? Schema { get; set; }
    public string? EndpointUrl { get; set; }
    public string? Authentication { get; set; }
    public string? Parameters { get; set; }
    public string? ReturnType { get; set; }
    public bool RequiresApproval { get; set; } = false;
    public int? TimeoutSeconds { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<ToolExecution>? Executions { get; set; }
}
