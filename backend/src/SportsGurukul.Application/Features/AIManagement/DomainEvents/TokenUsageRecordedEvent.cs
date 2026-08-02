using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record TokenUsageRecordedEvent(
    Guid TokenUsageId,
    Guid? ConversationId,
    string ModelName,
    int TotalTokens,
    decimal? Cost,
    DateTime RecordedAt
) : INotification;
