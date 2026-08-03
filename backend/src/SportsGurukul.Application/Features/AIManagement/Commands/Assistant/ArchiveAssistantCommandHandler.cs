using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class ArchiveAssistantCommandHandler : IRequestHandler<ArchiveAssistantCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public ArchiveAssistantCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public Task<Result<AssistantDto>> Handle(ArchiveAssistantCommand request, CancellationToken cancellationToken)
        => _assistantService.ArchiveAsync(request.AssistantId, cancellationToken);
}
