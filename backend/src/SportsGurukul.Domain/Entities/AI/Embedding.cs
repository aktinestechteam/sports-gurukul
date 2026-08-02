using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class Embedding : BaseEntity
{
    public Guid? DocumentId { get; set; }
    public Guid? ChunkId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int Dimensions { get; set; }
    public float[] Vector { get; set; } = [];
    public string? Text { get; set; }
    public int? TokenCount { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeDocument? Document { get; set; }
    public EmbeddingChunk? Chunk { get; set; }
}
