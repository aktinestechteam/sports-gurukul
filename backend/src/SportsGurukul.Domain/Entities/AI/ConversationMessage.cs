using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ConversationMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public int SequenceNumber { get; set; }
    public AIMessageRole Role { get; set; }
    public AIMessageContentType ContentType { get; set; } = AIMessageContentType.Text;
    public string Content { get; set; } = string.Empty;
    public string? ModelName { get; set; }
    public int? PromptVersionUsed { get; set; }
    public int? InputTokenCount { get; set; }
    public int? OutputTokenCount { get; set; }
    public long? LatencyMs { get; set; }
    public string? ToolCallsJson { get; set; }
    public string? ToolResultsJson { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Conversation? Conversation { get; set; }
}
