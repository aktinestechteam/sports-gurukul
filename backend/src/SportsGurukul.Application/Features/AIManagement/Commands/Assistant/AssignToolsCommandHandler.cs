using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class AssignToolsCommandHandler : IRequestHandler<AssignToolsCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public AssignToolsCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public async Task<Result<AssistantDto>> Handle(AssignToolsCommand request, CancellationToken cancellationToken)
    {
        var assignRequest = new AssignToolsRequest(
            request.AssistantId,
            request.ToolDefinitionIds,
            request.ClearExisting);

        return await _assistantService.AssignToolsAsync(assignRequest, cancellationToken);
    }
}
