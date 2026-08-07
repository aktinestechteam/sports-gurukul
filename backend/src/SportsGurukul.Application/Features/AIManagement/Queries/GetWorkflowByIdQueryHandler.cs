using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetWorkflowByIdQueryHandler : IRequestHandler<GetWorkflowByIdQuery, Result<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;

    public GetWorkflowByIdQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public Task<Result<WorkflowDto>> Handle(GetWorkflowByIdQuery request, CancellationToken cancellationToken)
        => _workflowService.GetByIdAsync(request.WorkflowId, cancellationToken);
}
