using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record PublishPromptTemplateCommand(
    Guid PromptTemplateId,
    string? ChangeSummary,
    string? Notes
) : IRequest<Result<PromptTemplateDto>>;
