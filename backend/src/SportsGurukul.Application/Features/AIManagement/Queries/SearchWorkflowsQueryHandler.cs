using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchWorkflowsQueryHandler
    : IRequestHandler<SearchWorkflowsQuery, Result<PaginatedResult<WorkflowSummaryDto>>>
{
    private readonly IWorkflowDefinitionRepository _workflowRepo;

    public SearchWorkflowsQueryHandler(IWorkflowDefinitionRepository workflowRepo)
    {
        _workflowRepo = workflowRepo;
    }

    public async Task<Result<PaginatedResult<WorkflowSummaryDto>>> Handle(SearchWorkflowsQuery request, CancellationToken cancellationToken)
    {
        var query = await _workflowRepo.FindAsync(w => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(w =>
                w.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (w.Description != null && w.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.Status.HasValue)
            filtered = filtered.Where(w => w.Status == request.Status.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(w => new WorkflowSummaryDto(
                w.Id, w.Name, w.Description, w.Status, w.Version, w.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<WorkflowSummaryDto>>.Success(
            new PaginatedResult<WorkflowSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
