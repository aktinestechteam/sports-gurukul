using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchAgentsQueryHandler
    : IRequestHandler<SearchAgentsQuery, Result<PaginatedResult<AgentSummaryDto>>>
{
    private readonly IAgentDefinitionRepository _agentRepo;

    public SearchAgentsQueryHandler(IAgentDefinitionRepository agentRepo)
    {
        _agentRepo = agentRepo;
    }

    public async Task<Result<PaginatedResult<AgentSummaryDto>>> Handle(SearchAgentsQuery request, CancellationToken cancellationToken)
    {
        var query = await _agentRepo.FindAsync(a => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(a =>
                a.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (a.Description != null && a.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.Status.HasValue)
            filtered = filtered.Where(a => a.Status == request.Status.Value);

        if (request.AssistantId.HasValue)
            filtered = filtered.Where(a => a.AssistantId == request.AssistantId.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AgentSummaryDto(
                a.Id, a.Name, a.Description, a.AssistantId, a.Status, a.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<AgentSummaryDto>>.Success(
            new PaginatedResult<AgentSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
