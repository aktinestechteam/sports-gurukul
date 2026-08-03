using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeSource : BaseEntity
{
    public Guid KnowledgeBaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIKnowledgeSourceType SourceType { get; set; }
    public string? Uri { get; set; }
    public string? ExternalId { get; set; }
    public string? ContentType { get; set; }
    public AIIngestionStatus IngestionStatus { get; set; } = AIIngestionStatus.Pending;
    public string? StatusMessage { get; set; }
    public DateTime? LastIngestedAt { get; set; }
    public int? RefreshIntervalMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public string? MetadataJson { get; set; }
    public string? ErrorDetailsJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeBase? KnowledgeBase { get; set; }
    public ICollection<KnowledgeDocument> Documents { get; set; } = new List<KnowledgeDocument>();
}
