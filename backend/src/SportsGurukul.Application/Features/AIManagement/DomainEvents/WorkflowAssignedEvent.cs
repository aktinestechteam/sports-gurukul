using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record WorkflowAssignedEvent(
    Guid AgentId,
    Guid WorkflowDefinitionId,
    DateTime AssignedAt
) : INotification;
