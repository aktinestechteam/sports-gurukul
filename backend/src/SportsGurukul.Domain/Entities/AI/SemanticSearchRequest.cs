using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class SemanticSearchRequest : BaseEntity
{
    public string Query { get; set; } = string.Empty;
    public Guid? KnowledgeBaseId { get; set; }
    public Guid? IndexId { get; set; }
    public int MaxResults { get; set; } = 10;
    public double MinScore { get; set; } = 0.7;
    public string? ModelName { get; set; }
    public string? Filters { get; set; }
    public SemanticSearchStatus Status { get; set; } = SemanticSearchStatus.Pending;
    public int? ResultCount { get; set; }
    public double? ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<SemanticSearchResult>? Results { get; set; }
}
