using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class AssignKnowledgeBaseCommandHandler : IRequestHandler<AssignKnowledgeBaseCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public AssignKnowledgeBaseCommandHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public async Task<Result<AssistantDto>> Handle(AssignKnowledgeBaseCommand request, CancellationToken cancellationToken)
    {
        var assignRequest = new AssignKnowledgeBaseRequest(
            request.AssistantId,
            request.KnowledgeBaseIds,
            request.ClearExisting);

        return await _assistantService.AssignKnowledgeBaseAsync(assignRequest, cancellationToken);
    }
}
