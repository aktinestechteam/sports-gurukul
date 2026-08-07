using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record ClonePromptCommand(
    Guid SourcePromptId,
    Guid? TargetAssistantId,
    string? NewName
) : IRequest<Result<PromptTemplateDto>>;
