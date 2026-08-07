using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record CreateKnowledgeBaseCommand(
    string Name,
    string? Description,
    AIKnowledgeBaseType KnowledgeBaseType,
    AIResourceOwnerType OwnerType,
    Guid? OwnerUserId,
    Guid? EmbeddingModelId,
    Guid? VectorIndexId,
    AIChunkingStrategy ChunkingStrategy,
    int ChunkSize,
    int ChunkOverlap,
    string? MetadataSchemaJson
) : IRequest<Result<KnowledgeBaseDto>>;
