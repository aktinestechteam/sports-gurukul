using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Observability;

public class ObservabilityService : IObservabilityService
{
    private readonly IMetricsCollector _collector;
    private readonly IMetricsReporter _reporter;

    public ObservabilityService(IMetricsCollector collector, IMetricsReporter reporter)
    {
        _collector = collector;
        _reporter = reporter;
    }

    public IMetricsCollector Collector => _collector;

    public Task<MetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default) =>
        _reporter.GetSnapshotAsync(cancellationToken);

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = await _reporter.GetSnapshotAsync(cancellationToken);
        if (snapshot is null)
        {
            return false;
        }

        var totalCalls = snapshot.Agents.Sum(a => a.TotalCalls)
            + snapshot.Tools.Sum(t => t.TotalCalls)
            + snapshot.Workflows.Sum(w => w.TotalExecutions);

        if (totalCalls == 0)
        {
            return true;
        }

        var failureRate = (double)snapshot.TotalFailures / totalCalls;
        return failureRate < 0.5;
    }
}
