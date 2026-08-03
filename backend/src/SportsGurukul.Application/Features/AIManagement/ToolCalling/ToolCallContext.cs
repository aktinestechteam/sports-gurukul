namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public record ToolCallContext(
    Guid? AgentId,
    Guid? ConversationId,
    Guid? UserId,
    string? CorrelationId,
    IReadOnlyDictionary<string, string>? Metadata
);
