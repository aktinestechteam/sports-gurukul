using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class PublishAssistantCommandHandler : IRequestHandler<PublishAssistantCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public PublishAssistantCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public Task<Result<AssistantDto>> Handle(PublishAssistantCommand request, CancellationToken cancellationToken)
        => _assistantService.PublishAsync(request.AssistantId, cancellationToken);
}
