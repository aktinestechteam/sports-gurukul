using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record ConversationCreatedEvent(
    Guid ConversationId,
    Guid? AssistantId,
    Guid? UserId,
    DateTime CreatedAt
) : INotification;
