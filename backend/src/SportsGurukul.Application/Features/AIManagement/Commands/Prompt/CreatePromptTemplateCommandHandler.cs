using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class CreatePromptTemplateCommandHandler : IRequestHandler<CreatePromptTemplateCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public CreatePromptTemplateCommandHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(CreatePromptTemplateCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreatePromptTemplateRequest(
            request.AssistantId,
            request.Name,
            request.Description,
            request.PromptType,
            request.TemplateText,
            request.InputSchemaJson,
            request.OutputSchemaJson,
            request.VariablesJson,
            request.IsDefault);

        return await _promptService.CreateAsync(createRequest, cancellationToken);
    }
}
