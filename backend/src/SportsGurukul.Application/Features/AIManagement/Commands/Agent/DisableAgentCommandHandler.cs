using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class DisableAgentCommandHandler : IRequestHandler<DisableAgentCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public DisableAgentCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public Task<Result<AgentDto>> Handle(DisableAgentCommand request, CancellationToken cancellationToken)
        => _agentService.DisableAsync(request.AgentId, cancellationToken);
}
