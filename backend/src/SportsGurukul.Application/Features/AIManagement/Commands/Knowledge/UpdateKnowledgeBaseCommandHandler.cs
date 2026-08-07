using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class UpdateKnowledgeBaseCommandHandler : IRequestHandler<UpdateKnowledgeBaseCommand, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public UpdateKnowledgeBaseCommandHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(UpdateKnowledgeBaseCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateKnowledgeBaseRequest(
            request.KnowledgeBaseId,
            request.Name,
            request.Description,
            request.KnowledgeBaseType,
            request.EmbeddingModelId,
            request.VectorIndexId,
            request.ChunkingStrategy,
            request.ChunkSize,
            request.ChunkOverlap,
            request.MetadataSchemaJson,
            request.IsActive,
            request.ExpectedRowVersion);

        return await _knowledgeService.UpdateAsync(updateRequest, cancellationToken);
    }
}
