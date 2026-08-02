using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.ModelRouting;

public record ModelSelectionRequest(
    AIModelCapability RequiredCapability,
    string? PreferredProvider,
    decimal? MaxCostPerToken,
    int? MaxLatencyMs,
    RoutingStrategy Strategy = RoutingStrategy.RoundRobin
);
