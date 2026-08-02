using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ConversationMemory : BaseEntity
{
    public Guid ConversationId { get; set; }
    public MemoryType Type { get; set; } = MemoryType.ShortTerm;
    public MemoryImportance Importance { get; set; } = MemoryImportance.Normal;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Keywords { get; set; }
    public string? Context { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsConsolidated { get; set; } = false;
    public double RelevanceScore { get; set; } = 0;
    public byte[] RowVersion { get; set; } = [];

    public Conversation Conversation { get; set; } = null!;
}
