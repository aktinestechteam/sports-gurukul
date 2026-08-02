using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeDocument : BaseEntity
{
    public Guid KnowledgeSourceId { get; set; }
    public DocumentType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? ContentType { get; set; }
    public int? PageCount { get; set; }
    public string? Content { get; set; }
    public string? Metadata { get; set; }
    public string? Checksum { get; set; }
    public EmbeddingStatus EmbeddingStatus { get; set; } = EmbeddingStatus.Pending;
    public DateTime? IndexedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeSource KnowledgeSource { get; set; } = null!;
    public ICollection<Embedding>? Embeddings { get; set; }
}
