using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AgentDefinition : BaseEntity
{
    public Guid? WorkflowId { get; set; }
    public Guid? ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIAgentType AgentType { get; set; }
    public string? SystemPrompt { get; set; }
    public double? Temperature { get; set; }
    public int? MaxIterations { get; set; }
    public bool MemoryEnabled { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ToolsJson { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public WorkflowDefinition? Workflow { get; set; }
    public AIModel? Model { get; set; }
    public ICollection<ToolDefinition> Tools { get; set; } = new List<ToolDefinition>();
    public ICollection<AgentExecution> Executions { get; set; } = new List<AgentExecution>();
    public ICollection<AIModelConfiguration> ModelConfigurations { get; set; } = new List<AIModelConfiguration>();
}
