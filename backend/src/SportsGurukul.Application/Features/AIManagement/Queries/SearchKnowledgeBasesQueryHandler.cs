using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchKnowledgeBasesQueryHandler : IRequestHandler<SearchKnowledgeBasesQuery, Result<IReadOnlyList<KnowledgeBaseDto>>>
{
    private readonly IKnowledgeService _knowledgeService;

    public SearchKnowledgeBasesQueryHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public Task<Result<IReadOnlyList<KnowledgeBaseDto>>> Handle(SearchKnowledgeBasesQuery request, CancellationToken cancellationToken)
        => _knowledgeService.SearchAsync(
            request.SearchTerm,
            request.KnowledgeBaseType,
            request.OwnerUserId,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);
}
