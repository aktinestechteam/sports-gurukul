using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class SemanticSearchRequest : BaseEntity
{
    public string Query { get; set; } = string.Empty;
    public float[]? QueryEmbedding { get; set; }
    public Guid? KnowledgeBaseId { get; set; }
    public Guid? VectorIndexId { get; set; }
    public Guid? ConversationId { get; set; }
    public int TopK { get; set; } = 5;
    public double? SimilarityThreshold { get; set; }
    public string? FiltersJson { get; set; }
    public string? ModelUsed { get; set; }
    public AISearchStatus Status { get; set; } = AISearchStatus.Pending;
    public int ResultCount { get; set; }
    public long? LatencyMs { get; set; }
    public AIResourceOwnerType RequestedByType { get; set; }
    public Guid? RequestedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeBase? KnowledgeBase { get; set; }
    public VectorIndex? VectorIndex { get; set; }
    public Conversation? Conversation { get; set; }
    public ICollection<SemanticSearchResult> Results { get; set; } = new List<SemanticSearchResult>();
}
