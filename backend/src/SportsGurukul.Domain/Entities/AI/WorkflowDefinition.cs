using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class WorkflowDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIWorkflowType WorkflowType { get; set; }
    public string DefinitionJson { get; set; } = "{}";
    public string? EntryNode { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool IsPublished { get; set; }
    public int? TimeoutSeconds { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<AgentDefinition> Agents { get; set; } = new List<AgentDefinition>();
    public ICollection<WorkflowExecution> Executions { get; set; } = new List<WorkflowExecution>();
}
