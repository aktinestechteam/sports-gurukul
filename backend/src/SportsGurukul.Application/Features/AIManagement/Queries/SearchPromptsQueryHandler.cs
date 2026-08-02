using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchPromptsQueryHandler
    : IRequestHandler<SearchPromptsQuery, Result<PaginatedResult<PromptSummaryDto>>>
{
    private readonly IPromptTemplateRepository _promptRepo;

    public SearchPromptsQueryHandler(IPromptTemplateRepository promptRepo)
    {
        _promptRepo = promptRepo;
    }

    public async Task<Result<PaginatedResult<PromptSummaryDto>>> Handle(SearchPromptsQuery request, CancellationToken cancellationToken)
    {
        var query = await _promptRepo.FindAsync(p => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(p =>
                p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (p.Description != null && p.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.Type.HasValue)
            filtered = filtered.Where(p => p.Type == request.Type.Value);

        if (request.Status.HasValue)
            filtered = filtered.Where(p => p.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Category))
            filtered = filtered.Where(p => p.Category != null && p.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PromptSummaryDto(
                p.Id, p.Name, p.Type, p.Status, p.CurrentVersion, p.Category, p.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<PromptSummaryDto>>.Success(
            new PaginatedResult<PromptSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
