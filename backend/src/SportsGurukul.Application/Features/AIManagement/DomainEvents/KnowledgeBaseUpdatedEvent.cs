using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record KnowledgeBaseUpdatedEvent(
    Guid KnowledgeBaseId,
    string Name,
    DateTime UpdatedAt
) : INotification;
