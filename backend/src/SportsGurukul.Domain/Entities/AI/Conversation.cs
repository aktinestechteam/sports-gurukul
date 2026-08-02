using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class Conversation : BaseEntity
{
    public string? Title { get; set; }
    public Guid? AssistantId { get; set; }
    public Guid? UserId { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public string? ContextSummary { get; set; }
    public int? TokenCount { get; set; }
    public int MessageCount { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIAssistant? Assistant { get; set; }
    public ICollection<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
    public ICollection<ConversationMemory>? Memories { get; set; }
}
