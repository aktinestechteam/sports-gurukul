using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class SemanticSearchResult : BaseEntity
{
    public Guid SearchRequestId { get; set; }
    public Guid? DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string? ChunkContent { get; set; }
    public double Score { get; set; }
    public int? Rank { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public SemanticSearchRequest SearchRequest { get; set; } = null!;
    public KnowledgeDocument? Document { get; set; }
}
