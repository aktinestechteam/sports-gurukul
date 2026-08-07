using MediatR;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record AgentCreatedEvent(
    Guid AgentId,
    string Name,
    AIAgentType AgentType,
    DateTime CreatedAt
) : INotification;
