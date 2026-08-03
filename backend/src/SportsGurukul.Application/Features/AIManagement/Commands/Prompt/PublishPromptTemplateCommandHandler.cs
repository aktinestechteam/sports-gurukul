using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class PublishPromptTemplateCommandHandler : IRequestHandler<PublishPromptTemplateCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public PublishPromptTemplateCommandHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(PublishPromptTemplateCommand request, CancellationToken cancellationToken)
    {
        var publishRequest = new PublishPromptTemplateRequest(request.PromptTemplateId, request.ChangeSummary, request.Notes);
        return await _promptService.PublishAsync(publishRequest, cancellationToken);
    }
}
