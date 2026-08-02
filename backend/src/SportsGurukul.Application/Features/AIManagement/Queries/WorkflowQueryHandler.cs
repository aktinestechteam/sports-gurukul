using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class WorkflowQueryHandler
    : IRequestHandler<WorkflowQuery, Result<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;

    public WorkflowQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public async Task<Result<WorkflowDto>> Handle(WorkflowQuery request, CancellationToken cancellationToken)
    {
        var result = await _workflowService.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<WorkflowDto>.Failure(result.Error!);

        var w = result.Value!;
        return Result<WorkflowDto>.Success(new WorkflowDto(
            w.Id, w.Name, w.Description, w.Status,
            w.Steps, w.Triggers, w.Conditions, w.Variables,
            w.Version, w.CreatedAt, w.UpdatedAt
        ));
    }
}
