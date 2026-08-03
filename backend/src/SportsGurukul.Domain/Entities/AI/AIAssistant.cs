using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIAssistant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIAssistantType AssistantType { get; set; }
    public string? SystemPrompt { get; set; }
    public Guid? ModelId { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? MaxTokens { get; set; }
    public bool MemoryEnabled { get; set; } = true;
    public bool StreamingEnabled { get; set; }
    public bool IsActive { get; set; } = true;
    public AIResourceOwnerType OwnerType { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? GuardrailsJson { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIModel? Model { get; set; }
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public ICollection<PromptTemplate> PromptTemplates { get; set; } = new List<PromptTemplate>();
    public ICollection<AIModelConfiguration> ModelConfigurations { get; set; } = new List<AIModelConfiguration>();
    public ICollection<AITokenUsage> TokenUsages { get; set; } = new List<AITokenUsage>();
}
