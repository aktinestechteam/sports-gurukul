using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class EmbeddingChunk : BaseEntity
{
    public Guid DocumentId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? TokenCount { get; set; }
    public int? CharacterCount { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeDocument Document { get; set; } = null!;
    public Embedding? Embedding { get; set; }
}
