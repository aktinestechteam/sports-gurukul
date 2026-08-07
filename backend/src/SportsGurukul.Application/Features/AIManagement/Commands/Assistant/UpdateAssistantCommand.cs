using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public record UpdateAssistantCommand(
    Guid AssistantId,
    string? Name,
    string? DisplayName,
    string? Description,
    AIAssistantType? AssistantType,
    string? SystemPrompt,
    Guid? ModelId,
    double? Temperature,
    double? TopP,
    int? MaxTokens,
    bool? MemoryEnabled,
    bool? StreamingEnabled,
    string? AvatarUrl,
    string? GuardrailsJson,
    byte[]? ExpectedRowVersion
) : IRequest<Result<AssistantDto>>;
