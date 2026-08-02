using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record MessageAddedEvent(
    Guid ConversationId,
    Guid MessageId,
    string Role,
    int TokenCount,
    DateTime CreatedAt
) : INotification;
