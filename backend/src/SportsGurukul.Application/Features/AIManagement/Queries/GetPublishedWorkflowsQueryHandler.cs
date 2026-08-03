using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetPublishedWorkflowsQueryHandler : IRequestHandler<GetPublishedWorkflowsQuery, Result<IReadOnlyList<WorkflowDto>>>
{
    private readonly IWorkflowService _workflowService;

    public GetPublishedWorkflowsQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public Task<Result<IReadOnlyList<WorkflowDto>>> Handle(GetPublishedWorkflowsQuery request, CancellationToken cancellationToken)
        => _workflowService.GetPublishedAsync(cancellationToken);
}
