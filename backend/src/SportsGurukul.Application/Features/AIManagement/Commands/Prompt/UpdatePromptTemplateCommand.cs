using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record UpdatePromptTemplateCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? TemplateContent,
    string? Variables,
    string? Tags,
    string? Category
) : IRequest<Result<PromptTemplateDto>>;
