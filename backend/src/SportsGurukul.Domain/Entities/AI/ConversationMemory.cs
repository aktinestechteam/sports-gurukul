using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ConversationMemory : BaseEntity
{
    public Guid ConversationId { get; set; }
    public AIMemoryType MemoryType { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Importance { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Conversation? Conversation { get; set; }
}
