using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchAgentsQueryHandler : IRequestHandler<SearchAgentsQuery, Result<IReadOnlyList<AgentDto>>>
{
    private readonly IAgentService _agentService;

    public SearchAgentsQueryHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public Task<Result<IReadOnlyList<AgentDto>>> Handle(SearchAgentsQuery request, CancellationToken cancellationToken)
        => _agentService.SearchAsync(
            request.SearchTerm,
            request.AgentType,
            request.WorkflowId,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);
}
