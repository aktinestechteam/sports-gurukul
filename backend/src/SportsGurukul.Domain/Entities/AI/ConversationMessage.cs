using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ConversationMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public MessageRole Role { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public string Content { get; set; } = string.Empty;
    public string? PromptTokens { get; set; }
    public string? CompletionTokens { get; set; }
    public int? TotalTokens { get; set; }
    public int? TokensUsed { get; set; }
    public string? ToolCalls { get; set; }
    public string? ToolResults { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? Cost { get; set; }
    public double? LatencyMs { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Conversation Conversation { get; set; } = null!;
}
