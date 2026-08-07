using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class UpdateAssistantCommandHandler : IRequestHandler<UpdateAssistantCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public UpdateAssistantCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public async Task<Result<AssistantDto>> Handle(UpdateAssistantCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateAssistantRequest(
            request.AssistantId,
            request.Name,
            request.DisplayName,
            request.Description,
            request.AssistantType,
            request.SystemPrompt,
            request.ModelId,
            request.Temperature,
            request.TopP,
            request.MaxTokens,
            request.MemoryEnabled,
            request.StreamingEnabled,
            request.AvatarUrl,
            request.GuardrailsJson,
            request.ExpectedRowVersion);

        return await _assistantService.UpdateAsync(updateRequest, cancellationToken);
    }
}
