using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AITokenUsage : BaseEntity
{
    public Guid? ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? ProviderName { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal? Cost { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? RequestType { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Conversation? Conversation { get; set; }
    public ConversationMessage? Message { get; set; }
}
