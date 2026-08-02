using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record ConversationArchivedEvent(
    Guid ConversationId,
    DateTime ArchivedAt
) : INotification;
