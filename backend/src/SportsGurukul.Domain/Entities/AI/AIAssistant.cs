using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIAssistant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIAssistantType AssistantType { get; set; } = AIAssistantType.General;
    public AIAssistantPersonality Personality { get; set; } = AIAssistantPersonality.Professional;
    public string? SystemPrompt { get; set; }
    public string? GreetingMessage { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public int? MaxHistoryLength { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<Conversation>? Conversations { get; set; }
    public ICollection<AgentDefinition>? AgentDefinitions { get; set; }
}
