using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AgentDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssistantId { get; set; }
    public AgentStatus Status { get; set; } = AgentStatus.Draft;
    public string? Configuration { get; set; }
    public string? Tools { get; set; }
    public string? Rules { get; set; }
    public string? Constraints { get; set; }
    public int MaxIterations { get; set; } = 10;
    public bool RequiresApproval { get; set; } = false;
    public byte[] RowVersion { get; set; } = [];

    public AIAssistant? Assistant { get; set; }
    public ICollection<AgentExecution>? Executions { get; set; }
}
