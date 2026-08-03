using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ToolDefinition : BaseEntity
{
    public Guid AgentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIToolType ToolType { get; set; }
    public string? Endpoint { get; set; }
    public string? HttpMethod { get; set; }
    public string InputSchemaJson { get; set; } = "{}";
    public string? OutputSchemaJson { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystemTool { get; set; }
    public int? TimeoutSeconds { get; set; }
    public bool RequiresApproval { get; set; }
    public string? RetryPolicyJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AgentDefinition? Agent { get; set; }
    public ICollection<ToolExecution> Executions { get; set; } = new List<ToolExecution>();
}
