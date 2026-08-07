using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record ConversationCreatedEvent(
    Guid ConversationId,
    Guid AssistantId,
    Guid? ParticipantUserId,
    DateTime CreatedAt
) : INotification;
