using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record SearchPromptTemplatesQuery(
    string? SearchTerm,
    Guid? AssistantId,
    AIPromptType? PromptType,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyList<PromptTemplateDto>>>;
