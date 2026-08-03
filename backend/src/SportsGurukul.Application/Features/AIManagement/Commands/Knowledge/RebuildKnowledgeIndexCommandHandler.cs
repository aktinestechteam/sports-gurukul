using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class RebuildKnowledgeIndexCommandHandler : IRequestHandler<RebuildKnowledgeIndexCommand, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public RebuildKnowledgeIndexCommandHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(RebuildKnowledgeIndexCommand request, CancellationToken cancellationToken)
    {
        var rebuildRequest = new RebuildKnowledgeIndexRequest(request.KnowledgeBaseId);
        return await _knowledgeService.RebuildIndexAsync(rebuildRequest, cancellationToken);
    }
}
