using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record ConversationArchivedEvent(
    Guid ConversationId,
    Guid AssistantId,
    DateTime ArchivedAt
) : INotification;
