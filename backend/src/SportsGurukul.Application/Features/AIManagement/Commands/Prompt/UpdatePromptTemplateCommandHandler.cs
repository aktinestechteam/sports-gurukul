using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class UpdatePromptTemplateCommandHandler : IRequestHandler<UpdatePromptTemplateCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public UpdatePromptTemplateCommandHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(UpdatePromptTemplateCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdatePromptTemplateRequest(
            request.PromptTemplateId,
            request.Name,
            request.Description,
            request.TemplateText,
            request.InputSchemaJson,
            request.OutputSchemaJson,
            request.VariablesJson,
            request.IsActive,
            request.ExpectedRowVersion);

        return await _promptService.UpdateAsync(updateRequest, cancellationToken);
    }
}
