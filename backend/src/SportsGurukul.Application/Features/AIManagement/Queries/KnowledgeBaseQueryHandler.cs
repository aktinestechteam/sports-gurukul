using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class KnowledgeBaseQueryHandler
    : IRequestHandler<KnowledgeBaseQuery, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public KnowledgeBaseQueryHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(KnowledgeBaseQuery request, CancellationToken cancellationToken)
    {
        var result = await _knowledgeService.GetBaseByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<KnowledgeBaseDto>.Failure(result.Error!);

        var kb = result.Value!;
        return Result<KnowledgeBaseDto>.Success(new KnowledgeBaseDto(
            kb.Id, kb.Name, kb.Description, kb.Visibility, kb.Status,
            kb.Category, kb.Tags, kb.IconUrl,
            kb.TotalSources, kb.TotalDocuments, kb.TotalSizeBytes,
            kb.CreatedAt, kb.UpdatedAt,
            kb.Sources?.Select(s => new KnowledgeSourceSummaryDto(
                s.Id, s.KnowledgeBaseId, s.Name, s.SourceType, s.Status,
                s.DocumentCount, s.LastSyncAt
            )).ToList() ?? null
        ));
    }
}
