using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record CreatePromptTemplateCommand(
    Guid AssistantId,
    string Name,
    string? Description,
    AIPromptType PromptType,
    string TemplateText,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    string? VariablesJson,
    bool IsDefault
) : IRequest<Result<PromptTemplateDto>>;
