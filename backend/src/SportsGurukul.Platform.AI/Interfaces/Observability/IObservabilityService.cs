using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Observability;

public interface IObservabilityService
{
    IMetricsCollector Collector { get; }

    Task<MetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);

    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}
