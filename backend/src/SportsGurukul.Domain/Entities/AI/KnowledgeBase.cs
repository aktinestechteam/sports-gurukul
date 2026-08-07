using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeBase : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIKnowledgeBaseType KnowledgeBaseType { get; set; }
    public AIResourceOwnerType OwnerType { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? VectorIndexId { get; set; }
    public Guid? EmbeddingModelId { get; set; }
    public AIChunkingStrategy ChunkingStrategy { get; set; } = AIChunkingStrategy.Recursive;
    public int ChunkSize { get; set; } = 1000;
    public int ChunkOverlap { get; set; } = 100;
    public int EmbeddingDimension { get; set; }
    public bool IsActive { get; set; } = true;
    public string? MetadataSchemaJson { get; set; }
    public string? StatisticsJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public VectorIndex? VectorIndex { get; set; }
    public AIModel? EmbeddingModel { get; set; }
    public ICollection<KnowledgeSource> Sources { get; set; } = new List<KnowledgeSource>();
    public ICollection<KnowledgeDocument> Documents { get; set; } = new List<KnowledgeDocument>();
    public ICollection<Embedding> Embeddings { get; set; } = new List<Embedding>();
    public ICollection<EmbeddingChunk> Chunks { get; set; } = new List<EmbeddingChunk>();
    public ICollection<SemanticSearchRequest> SearchRequests { get; set; } = new List<SemanticSearchRequest>();
}
