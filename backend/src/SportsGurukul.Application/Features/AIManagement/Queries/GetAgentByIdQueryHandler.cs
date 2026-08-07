using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public GetAgentByIdQueryHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public Task<Result<AgentDto>> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        => _agentService.GetByIdAsync(request.AgentId, cancellationToken);
}
