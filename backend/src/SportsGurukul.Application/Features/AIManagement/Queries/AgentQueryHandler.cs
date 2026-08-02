using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class AgentQueryHandler
    : IRequestHandler<AgentQuery, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public AgentQueryHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public async Task<Result<AgentDto>> Handle(AgentQuery request, CancellationToken cancellationToken)
    {
        var result = await _agentService.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<AgentDto>.Failure(result.Error!);

        var a = result.Value!;
        return Result<AgentDto>.Success(new AgentDto(
            a.Id, a.Name, a.Description, a.AssistantId, a.Assistant?.Name,
            a.Status, a.Configuration, a.Tools, a.Rules, a.Constraints,
            a.MaxIterations, a.RequiresApproval,
            a.CreatedAt, a.UpdatedAt, null
        ));
    }
}
