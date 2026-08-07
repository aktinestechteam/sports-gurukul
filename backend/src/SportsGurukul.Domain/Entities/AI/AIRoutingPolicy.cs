using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIRoutingPolicy : BaseEntity
{
    public Guid? ProviderId { get; set; }
    public Guid? DefaultModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIRoutingStrategy RoutingStrategy { get; set; } = AIRoutingStrategy.Balanced;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ConditionsJson { get; set; }
    public string? PreferredModelIdsJson { get; set; }
    public string? FallbackModelIdsJson { get; set; }
    public double? MinScore { get; set; }
    public decimal? MaxCostPerRequest { get; set; }
    public int? MaxLatencyMs { get; set; }
    public bool AllowFallback { get; set; } = true;
    public int RetryCount { get; set; }
    public string? MetadataJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIProvider? Provider { get; set; }
    public AIModel? DefaultModel { get; set; }
}
