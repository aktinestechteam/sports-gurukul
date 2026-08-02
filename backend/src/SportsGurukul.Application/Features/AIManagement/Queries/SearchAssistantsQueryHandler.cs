using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchAssistantsQueryHandler
    : IRequestHandler<SearchAssistantsQuery, Result<PaginatedResult<AssistantSummaryDto>>>
{
    private readonly IAIAssistantRepository _assistantRepo;

    public SearchAssistantsQueryHandler(IAIAssistantRepository assistantRepo)
    {
        _assistantRepo = assistantRepo;
    }

    public async Task<Result<PaginatedResult<AssistantSummaryDto>>> Handle(SearchAssistantsQuery request, CancellationToken cancellationToken)
    {
        var query = await _assistantRepo.FindAsync(a => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(a =>
                a.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (a.Description != null && a.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.AssistantType.HasValue)
            filtered = filtered.Where(a => a.AssistantType == request.AssistantType.Value);

        if (request.IsActive.HasValue)
            filtered = filtered.Where(a => a.IsActive == request.IsActive.Value);

        if (request.IsPublic.HasValue)
            filtered = filtered.Where(a => a.IsPublic == request.IsPublic.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AssistantSummaryDto(
                a.Id, a.Name, a.Description, a.AssistantType, a.Personality,
                a.IsActive, a.IsPublic, a.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<AssistantSummaryDto>>.Success(
            new PaginatedResult<AssistantSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
