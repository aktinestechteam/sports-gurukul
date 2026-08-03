using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record TokenUsageRecordedEvent(
    Guid TokenUsageId,
    Guid? AssistantId,
    Guid? ConversationId,
    Guid? ModelId,
    int TotalTokens,
    decimal? Cost,
    DateTime OccurredAt
) : INotification;
