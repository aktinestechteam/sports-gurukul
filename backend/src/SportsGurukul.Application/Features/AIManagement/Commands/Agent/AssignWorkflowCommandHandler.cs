using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class AssignWorkflowCommandHandler : IRequestHandler<AssignWorkflowCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public AssignWorkflowCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public Task<Result<AgentDto>> Handle(AssignWorkflowCommand request, CancellationToken cancellationToken)
        => _agentService.AssignWorkflowAsync(request.AgentId, request.WorkflowId, cancellationToken);
}
