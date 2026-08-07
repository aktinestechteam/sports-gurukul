using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchWorkflowsQueryHandler : IRequestHandler<SearchWorkflowsQuery, Result<IReadOnlyList<WorkflowDto>>>
{
    private readonly IWorkflowService _workflowService;

    public SearchWorkflowsQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public Task<Result<IReadOnlyList<WorkflowDto>>> Handle(SearchWorkflowsQuery request, CancellationToken cancellationToken)
        => _workflowService.SearchAsync(
            request.SearchTerm,
            request.WorkflowType,
            request.IsActive,
            request.IsPublished,
            request.Page,
            request.PageSize,
            cancellationToken);
}
