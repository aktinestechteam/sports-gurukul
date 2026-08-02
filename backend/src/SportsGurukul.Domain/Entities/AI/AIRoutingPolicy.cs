using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIRoutingPolicy : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RoutingStrategy Strategy { get; set; } = RoutingStrategy.RoundRobin;
    public RoutingStatus Status { get; set; } = RoutingStatus.Active;
    public string? ProviderIds { get; set; }
    public string? ModelIds { get; set; }
    public string? Rules { get; set; }
    public int? Priority { get; set; }
    public int MaxRetries { get; set; } = 3;
    public bool FallbackEnabled { get; set; } = true;
    public string? FallbackPolicy { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
