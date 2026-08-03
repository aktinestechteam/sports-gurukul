using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record UpdateKnowledgeBaseCommand(
    Guid KnowledgeBaseId,
    string? Name,
    string? Description,
    AIKnowledgeBaseType? KnowledgeBaseType,
    Guid? EmbeddingModelId,
    Guid? VectorIndexId,
    AIChunkingStrategy? ChunkingStrategy,
    int? ChunkSize,
    int? ChunkOverlap,
    string? MetadataSchemaJson,
    bool? IsActive,
    byte[]? ExpectedRowVersion
) : IRequest<Result<KnowledgeBaseDto>>;
