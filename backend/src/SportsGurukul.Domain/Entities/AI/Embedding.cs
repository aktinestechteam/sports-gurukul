using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class Embedding : BaseEntity
{
    public Guid ChunkId { get; set; }
    public Guid KnowledgeBaseId { get; set; }
    public Guid? ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public float[] Vector { get; set; } = [];
    public int Dimension { get; set; }
    public double Norm { get; set; }
    public AIEmbeddingStatus Status { get; set; } = AIEmbeddingStatus.Pending;
    public byte[] RowVersion { get; set; } = [];

    public EmbeddingChunk? Chunk { get; set; }
    public KnowledgeBase? KnowledgeBase { get; set; }
    public AIModel? Model { get; set; }
}
