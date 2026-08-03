using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public record CreateAssistantCommand(
    string Name,
    string DisplayName,
    string? Description,
    AIAssistantType AssistantType,
    string? SystemPrompt,
    Guid? ModelId,
    double? Temperature,
    double? TopP,
    int? MaxTokens,
    bool MemoryEnabled,
    bool StreamingEnabled,
    AIResourceOwnerType OwnerType,
    Guid? OwnerUserId,
    string? AvatarUrl,
    string? GuardrailsJson,
    string? MetadataJson
) : IRequest<Result<AssistantDto>>;
