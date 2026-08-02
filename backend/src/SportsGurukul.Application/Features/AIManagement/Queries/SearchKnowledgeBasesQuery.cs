using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record SearchKnowledgeBasesQuery(
    string? SearchTerm,
    KnowledgeBaseVisibility? Visibility,
    KnowledgeBaseStatus? Status,
    string? Category,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedResult<KnowledgeBaseSummaryDto>>>;
