using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetKnowledgeBaseByIdQueryHandler : IRequestHandler<GetKnowledgeBaseByIdQuery, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public GetKnowledgeBaseByIdQueryHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public Task<Result<KnowledgeBaseDto>> Handle(GetKnowledgeBaseByIdQuery request, CancellationToken cancellationToken)
        => _knowledgeService.GetByIdAsync(request.KnowledgeBaseId, cancellationToken);
}
