using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class PromptTemplate : BaseEntity
{
    public Guid AssistantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIPromptType PromptType { get; set; } = AIPromptType.Template;
    public string TemplateText { get; set; } = string.Empty;
    public string? InputSchemaJson { get; set; }
    public string? OutputSchemaJson { get; set; }
    public string? VariablesJson { get; set; }
    public int CurrentVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIAssistant? Assistant { get; set; }
    public ICollection<PromptVersion> Versions { get; set; } = new List<PromptVersion>();
}
