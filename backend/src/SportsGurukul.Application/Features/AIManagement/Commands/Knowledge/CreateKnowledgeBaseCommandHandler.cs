using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class CreateKnowledgeBaseCommandHandler : IRequestHandler<CreateKnowledgeBaseCommand, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public CreateKnowledgeBaseCommandHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(CreateKnowledgeBaseCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateKnowledgeBaseRequest(
            request.Name,
            request.Description,
            request.KnowledgeBaseType,
            request.OwnerType,
            request.OwnerUserId,
            request.EmbeddingModelId,
            request.VectorIndexId,
            request.ChunkingStrategy,
            request.ChunkSize,
            request.ChunkOverlap,
            request.MetadataSchemaJson);

        return await _knowledgeService.CreateAsync(createRequest, cancellationToken);
    }
}
