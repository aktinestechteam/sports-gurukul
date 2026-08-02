using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeSource : BaseEntity
{
    public Guid KnowledgeBaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public KnowledgeSourceType SourceType { get; set; }
    public SourceStatus Status { get; set; } = SourceStatus.Pending;
    public string? SourceUri { get; set; }
    public string? Configuration { get; set; }
    public string? Description { get; set; }
    public int DocumentCount { get; set; } = 0;
    public DateTime? LastSyncAt { get; set; }
    public string? ErrorMessage { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public KnowledgeBase KnowledgeBase { get; set; } = null!;
    public ICollection<KnowledgeDocument>? Documents { get; set; }
}
