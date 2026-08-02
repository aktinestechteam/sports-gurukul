using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record CreatePromptTemplateCommand(
    string Name,
    string? Description,
    PromptType Type,
    string TemplateContent,
    string? Variables,
    string? Tags,
    string? Category
) : IRequest<Result<PromptTemplateDto>>;
