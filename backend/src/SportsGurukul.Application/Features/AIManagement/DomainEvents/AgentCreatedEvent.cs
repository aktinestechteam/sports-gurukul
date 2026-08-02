using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record AgentCreatedEvent(
    Guid AgentId,
    string Name,
    Guid? AssistantId,
    DateTime CreatedAt
) : INotification;
