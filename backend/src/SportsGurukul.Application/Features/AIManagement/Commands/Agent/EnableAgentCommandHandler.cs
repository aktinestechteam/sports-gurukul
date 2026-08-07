using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class EnableAgentCommandHandler : IRequestHandler<EnableAgentCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public EnableAgentCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public Task<Result<AgentDto>> Handle(EnableAgentCommand request, CancellationToken cancellationToken)
        => _agentService.EnableAsync(request.AgentId, cancellationToken);
}
