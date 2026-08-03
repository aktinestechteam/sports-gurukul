using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class VectorIndex : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIVectorIndexProvider Provider { get; set; } = AIVectorIndexProvider.PgVector;
    public int Dimension { get; set; }
    public AIVectorDistanceMetric DistanceMetric { get; set; } = AIVectorDistanceMetric.Cosine;
    public AIVectorIndexStatus Status { get; set; } = AIVectorIndexStatus.Pending;
    public string? IndexName { get; set; }
    public long ItemCount { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ConfigurationJson { get; set; }
    public DateTime? LastIndexedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<KnowledgeBase> KnowledgeBases { get; set; } = new List<KnowledgeBase>();
    public ICollection<SemanticSearchRequest> SearchRequests { get; set; } = new List<SemanticSearchRequest>();
}
