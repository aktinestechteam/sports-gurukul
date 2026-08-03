using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class Conversation : BaseEntity
{
    public Guid AssistantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public AIConversationStatus Status { get; set; } = AIConversationStatus.Active;
    public AIResourceOwnerType ParticipantType { get; set; }
    public Guid? ParticipantUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int MessageCount { get; set; }
    public int TokenCount { get; set; }
    public string? KnowledgeBaseIdsJson { get; set; }
    public string? ContextMetadataJson { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIAssistant? Assistant { get; set; }
    public ICollection<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
    public ICollection<ConversationMemory> Memories { get; set; } = new List<ConversationMemory>();
    public ICollection<SemanticSearchRequest> SemanticSearchRequests { get; set; } = new List<SemanticSearchRequest>();
    public ICollection<AITokenUsage> TokenUsages { get; set; } = new List<AITokenUsage>();
}
