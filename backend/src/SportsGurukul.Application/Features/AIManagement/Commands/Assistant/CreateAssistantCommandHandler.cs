using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class CreateAssistantCommandHandler : IRequestHandler<CreateAssistantCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public CreateAssistantCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public async Task<Result<AssistantDto>> Handle(CreateAssistantCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateAssistantRequest(
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
            request.OwnerType,
            request.OwnerUserId,
            request.AvatarUrl,
            request.GuardrailsJson,
            request.MetadataJson);

        return await _assistantService.CreateAsync(createRequest, cancellationToken);
    }
}
