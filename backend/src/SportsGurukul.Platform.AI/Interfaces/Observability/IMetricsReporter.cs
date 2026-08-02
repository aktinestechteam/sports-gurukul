using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Observability;

public interface IMetricsReporter
{
    Task<MetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
