using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class ClonePromptCommandHandler : IRequestHandler<ClonePromptCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public ClonePromptCommandHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(ClonePromptCommand request, CancellationToken cancellationToken)
    {
        var cloneRequest = new ClonePromptRequest(request.SourcePromptId, request.TargetAssistantId, request.NewName);
        return await _promptService.CloneAsync(cloneRequest, cancellationToken);
    }
}
