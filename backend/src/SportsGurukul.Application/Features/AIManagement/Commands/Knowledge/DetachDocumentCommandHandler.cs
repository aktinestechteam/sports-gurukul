using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class DetachDocumentCommandHandler : IRequestHandler<DetachDocumentCommand, Result<bool>>
{
    private readonly IKnowledgeService _knowledgeService;

    public DetachDocumentCommandHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public Task<Result<bool>> Handle(DetachDocumentCommand request, CancellationToken cancellationToken)
        => _knowledgeService.DetachDocumentAsync(request.KnowledgeBaseId, request.DocumentId, cancellationToken);
}
