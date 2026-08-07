using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record UpdatePromptTemplateCommand(
    Guid PromptTemplateId,
    string? Name,
    string? Description,
    string? TemplateText,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    string? VariablesJson,
    bool? IsActive,
    byte[]? ExpectedRowVersion
) : IRequest<Result<PromptTemplateDto>>;
