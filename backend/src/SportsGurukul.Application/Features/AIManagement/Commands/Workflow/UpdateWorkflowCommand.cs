using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Workflow;

public record UpdateWorkflowCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Steps,
    string? Triggers,
    string? Conditions,
    string? Variables
) : IRequest<Result<WorkflowDto>>;
