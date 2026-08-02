using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public record AssignWorkflowCommand(
    Guid AgentId,
    Guid WorkflowDefinitionId
) : IRequest<Result<AgentDto>>;
