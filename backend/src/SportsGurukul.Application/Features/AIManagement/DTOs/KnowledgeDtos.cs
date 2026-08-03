using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record KnowledgeBaseDto(
    Guid Id,
    string Name,
    string? Description,
    AIKnowledgeBaseType KnowledgeBaseType,
    AIResourceOwnerType OwnerType,
    Guid? OwnerUserId,
    Guid? VectorIndexId,
    Guid? EmbeddingModelId,
    AIChunkingStrategy ChunkingStrategy,
    int ChunkSize,
    int ChunkOverlap,
    int EmbeddingDimension,
    bool IsActive,
    int DocumentCount,
    string? StatisticsJson,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record KnowledgeDocumentDto(
    Guid Id,
    Guid KnowledgeBaseId,
    Guid? KnowledgeSourceId,
    string Title,
    AIKnowledgeDocumentType DocumentType,
    string ContentHash,
    string? ExternalId,
    string? StoragePath,
    string? MimeType,
    int? PageCount,
    int? WordCount,
    AIDocumentStatus Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt
);

public record CreateKnowledgeBaseRequest(
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
);

public record UpdateKnowledgeBaseRequest(
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
);

public record AttachDocumentRequest(
    Guid KnowledgeBaseId,
    string Title,
    AIKnowledgeDocumentType DocumentType,
    string? Content,
    string? ExternalId,
    string? StoragePath,
    string? MimeType,
    string? MetadataJson
);

public record RebuildKnowledgeIndexRequest(
    Guid KnowledgeBaseId
);
