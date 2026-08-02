using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class KnowledgeBase : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public KnowledgeBaseVisibility Visibility { get; set; } = KnowledgeBaseVisibility.Private;
    public KnowledgeBaseStatus Status { get; set; } = KnowledgeBaseStatus.Draft;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? IconUrl { get; set; }
    public int TotalSources { get; set; } = 0;
    public int TotalDocuments { get; set; } = 0;
    public long? TotalSizeBytes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<KnowledgeSource>? Sources { get; set; }
}
