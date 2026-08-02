using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record SearchPromptsQuery(
    string? SearchTerm,
    PromptType? Type,
    PromptStatus? Status,
    string? Category,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedResult<PromptSummaryDto>>>;
