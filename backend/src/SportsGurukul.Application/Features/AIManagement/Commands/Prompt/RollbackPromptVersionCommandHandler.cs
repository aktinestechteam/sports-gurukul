using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class RollbackPromptVersionCommandHandler : IRequestHandler<RollbackPromptVersionCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public RollbackPromptVersionCommandHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(RollbackPromptVersionCommand request, CancellationToken cancellationToken)
    {
        var rollbackRequest = new RollbackPromptVersionRequest(request.PromptTemplateId, request.VersionNumber);
        return await _promptService.RollbackAsync(rollbackRequest, cancellationToken);
    }
}
