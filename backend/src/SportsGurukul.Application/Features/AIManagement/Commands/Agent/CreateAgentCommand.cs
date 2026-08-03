using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public record CreateAgentCommand(
    Guid? WorkflowId,
    Guid? ModelId,
    string Name,
    string? Description,
    AIAgentType AgentType,
    string? SystemPrompt,
    double? Temperature,
    int? MaxIterations,
    bool MemoryEnabled,
    string? ToolsJson,
    string? MetadataJson
) : IRequest<Result<AgentDto>>;
