using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AITokenUsage : BaseEntity
{
    public Guid? ProviderId { get; set; }
    public Guid? ModelId { get; set; }
    public Guid? AssistantId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? UserId { get; set; }
    public AIResourceOwnerType UserType { get; set; }
    public AIUsageType UsageType { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public int? CacheReadTokens { get; set; }
    public int? CacheWriteTokens { get; set; }
    public decimal? Cost { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public long? LatencyMs { get; set; }
    public string? ModelName { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIProvider? Provider { get; set; }
    public AIModel? Model { get; set; }
    public AIAssistant? Assistant { get; set; }
    public Conversation? Conversation { get; set; }
}
