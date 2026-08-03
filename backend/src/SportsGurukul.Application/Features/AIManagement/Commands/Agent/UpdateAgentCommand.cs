using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public record UpdateAgentCommand(
    Guid AgentId,
    string? Name,
    string? Description,
    AIAgentType? AgentType,
    string? SystemPrompt,
    double? Temperature,
    int? MaxIterations,
    bool? MemoryEnabled,
    Guid? ModelId,
    string? ToolsJson,
    byte[]? ExpectedRowVersion
) : IRequest<Result<AgentDto>>;
