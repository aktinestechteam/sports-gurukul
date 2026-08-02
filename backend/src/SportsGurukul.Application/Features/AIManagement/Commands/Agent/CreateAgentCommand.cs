using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public record CreateAgentCommand(
    string Name,
    string? Description,
    Guid? AssistantId,
    string? Configuration,
    string? Tools,
    string? Rules,
    string? Constraints,
    int? MaxIterations,
    bool? RequiresApproval
) : IRequest<Result<AgentDto>>;
