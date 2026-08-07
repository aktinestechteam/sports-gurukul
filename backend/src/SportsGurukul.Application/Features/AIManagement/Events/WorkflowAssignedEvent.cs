using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record WorkflowAssignedEvent(
    Guid AgentId,
    Guid WorkflowId,
    DateTime AssignedAt
) : INotification;
