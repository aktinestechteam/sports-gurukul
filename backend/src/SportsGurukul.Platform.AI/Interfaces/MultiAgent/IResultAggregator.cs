using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.MultiAgent;

public interface IResultAggregator
{
    Task<AggregationResult> AggregateAsync(IReadOnlyList<DelegatedTaskResult> results, AggregationStrategy strategy, CancellationToken cancellationToken = default);
}
