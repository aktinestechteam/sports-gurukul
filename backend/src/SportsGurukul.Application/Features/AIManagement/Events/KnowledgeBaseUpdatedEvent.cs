using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record KnowledgeBaseUpdatedEvent(
    Guid KnowledgeBaseId,
    string Name,
    DateTime UpdatedAt
) : INotification;
