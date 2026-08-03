using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.AI;

public class EmbeddingChunk : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Guid KnowledgeBaseId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? TokenCount { get; set; }
    public int CharacterCount { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeDocument? Document { get; set; }
    public KnowledgeBase? KnowledgeBase { get; set; }
    public Embedding? Embedding { get; set; }
    public ICollection<SemanticSearchResult> SearchResults { get; set; } = new List<SemanticSearchResult>();
}
