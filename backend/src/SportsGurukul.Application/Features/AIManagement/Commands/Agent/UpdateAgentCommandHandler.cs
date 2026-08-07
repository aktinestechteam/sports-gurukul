using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public UpdateAgentCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public async Task<Result<AgentDto>> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateAgentRequest(
            request.AgentId,
            request.Name,
            request.Description,
            request.AgentType,
            request.SystemPrompt,
            request.Temperature,
            request.MaxIterations,
            request.MemoryEnabled,
            request.ModelId,
            request.ToolsJson,
            request.ExpectedRowVersion);

        return await _agentService.UpdateAsync(updateRequest, cancellationToken);
    }
}
