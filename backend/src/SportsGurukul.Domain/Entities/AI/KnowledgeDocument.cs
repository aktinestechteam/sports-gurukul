using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeDocument : BaseEntity
{
    public Guid KnowledgeBaseId { get; set; }
    public Guid? KnowledgeSourceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public AIKnowledgeDocumentType DocumentType { get; set; }
    public string? Content { get; set; }
    public string ContentHash { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? StoragePath { get; set; }
    public string? MimeType { get; set; }
    public int? PageCount { get; set; }
    public int? WordCount { get; set; }
    public AIDocumentStatus Status { get; set; } = AIDocumentStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeBase? KnowledgeBase { get; set; }
    public KnowledgeSource? KnowledgeSource { get; set; }
    public ICollection<EmbeddingChunk> Chunks { get; set; } = new List<EmbeddingChunk>();
    public ICollection<SemanticSearchResult> SearchResults { get; set; } = new List<SemanticSearchResult>();
}
