using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchKnowledgeBasesQueryHandler
    : IRequestHandler<SearchKnowledgeBasesQuery, Result<PaginatedResult<KnowledgeBaseSummaryDto>>>
{
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepo;

    public SearchKnowledgeBasesQueryHandler(IKnowledgeBaseRepository knowledgeBaseRepo)
    {
        _knowledgeBaseRepo = knowledgeBaseRepo;
    }

    public async Task<Result<PaginatedResult<KnowledgeBaseSummaryDto>>> Handle(SearchKnowledgeBasesQuery request, CancellationToken cancellationToken)
    {
        var query = await _knowledgeBaseRepo.FindAsync(kb => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(kb =>
                kb.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (kb.Description != null && kb.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.Visibility.HasValue)
            filtered = filtered.Where(kb => kb.Visibility == request.Visibility.Value);

        if (request.Status.HasValue)
            filtered = filtered.Where(kb => kb.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Category))
            filtered = filtered.Where(kb => kb.Category != null && kb.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(kb => new KnowledgeBaseSummaryDto(
                kb.Id, kb.Name, kb.Description, kb.Visibility, kb.Status,
                kb.TotalDocuments, kb.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<KnowledgeBaseSummaryDto>>.Success(
            new PaginatedResult<KnowledgeBaseSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
