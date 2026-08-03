using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetKnowledgeBaseDocumentsQueryHandler : IRequestHandler<GetKnowledgeBaseDocumentsQuery, Result<IReadOnlyList<KnowledgeDocumentDto>>>
{
    private readonly IKnowledgeService _knowledgeService;

    public GetKnowledgeBaseDocumentsQueryHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public Task<Result<IReadOnlyList<KnowledgeDocumentDto>>> Handle(GetKnowledgeBaseDocumentsQuery request, CancellationToken cancellationToken)
        => _knowledgeService.GetDocumentsAsync(request.KnowledgeBaseId, cancellationToken);
}
