using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class AssignWorkflowCommandHandler : IRequestHandler<AssignWorkflowCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignWorkflowCommandHandler(IAgentService agentService, IUnitOfWork unitOfWork)
    {
        _agentService = agentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AgentDto>> Handle(AssignWorkflowCommand request, CancellationToken cancellationToken)
    {
        var result = await _agentService.AssignWorkflowAsync(request.AgentId, request.WorkflowDefinitionId, cancellationToken);
        if (!result.IsSuccess)
            return Result<AgentDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AgentDto>.Success(default!);
    }
}
