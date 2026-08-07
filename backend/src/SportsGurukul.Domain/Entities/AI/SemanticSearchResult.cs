using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.AI;

public class SemanticSearchResult : BaseEntity
{
    public Guid SemanticSearchRequestId { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? ChunkId { get; set; }
    public double Score { get; set; }
    public int Rank { get; set; }
    public string? Content { get; set; }
    public string? MetadataJson { get; set; }
    public double? ReRankScore { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public SemanticSearchRequest? SemanticSearchRequest { get; set; }
    public KnowledgeDocument? Document { get; set; }
    public EmbeddingChunk? Chunk { get; set; }
}
