using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public record UpdateAssistantCommand(
    Guid Id,
    string? Name,
    string? Description,
    AIAssistantType? AssistantType,
    AIAssistantPersonality? Personality,
    string? SystemPrompt,
    string? GreetingMessage,
    bool? IsPublic
) : IRequest<Result<AssistantDto>>;
