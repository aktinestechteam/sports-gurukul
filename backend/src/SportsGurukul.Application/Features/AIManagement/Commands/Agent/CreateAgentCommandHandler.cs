using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;

    public CreateAgentCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public async Task<Result<AgentDto>> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateAgentRequest(
            request.WorkflowId,
            request.ModelId,
            request.Name,
            request.Description,
            request.AgentType,
            request.SystemPrompt,
            request.Temperature,
            request.MaxIterations,
            request.MemoryEnabled,
            request.ToolsJson,
            request.MetadataJson);

        return await _agentService.CreateAsync(createRequest, cancellationToken);
    }
}
