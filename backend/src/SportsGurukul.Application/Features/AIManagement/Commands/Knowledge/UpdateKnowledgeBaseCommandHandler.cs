using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class UpdateKnowledgeBaseCommandHandler : IRequestHandler<UpdateKnowledgeBaseCommand, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateKnowledgeBaseCommandHandler(IKnowledgeService knowledgeService, IUnitOfWork unitOfWork)
    {
        _knowledgeService = knowledgeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(UpdateKnowledgeBaseCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateKnowledgeBaseRequest(
            request.Id, request.Name, request.Description, request.Visibility, request.Category, request.Tags);
        var result = await _knowledgeService.UpdateBaseAsync(updateRequest, cancellationToken);
        if (!result.IsSuccess)
            return Result<KnowledgeBaseDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var kb = result.Value!;
        return Result<KnowledgeBaseDto>.Success(new KnowledgeBaseDto(
            kb.Id, kb.Name, kb.Description, kb.Visibility, kb.Status,
            kb.Category, kb.Tags, kb.IconUrl, kb.TotalSources, kb.TotalDocuments,
            kb.TotalSizeBytes, kb.CreatedAt, kb.UpdatedAt,
            kb.Sources?.Select(s => new KnowledgeSourceSummaryDto(
                s.Id, s.KnowledgeBaseId, s.Name, s.SourceType, s.Status, s.DocumentCount, s.LastSyncAt
            )).ToList()
        ));
    }
}
