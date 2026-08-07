using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class LatencyBasedModelSelectionStrategy : IModelSelectionStrategy
{
    public AIRoutingStrategy Strategy => AIRoutingStrategy.Speed;

    public Task<Result<ModelSelectionResult>> SelectAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var eligible = candidates
            .Where(c => ModelSelectionCalculator.MeetsCapabilities(c, context))
            .Where(c => context.MaxLatencyMs is null
                || (ModelSelectionCalculator.EstimateLatency(c) ?? int.MaxValue) <= context.MaxLatencyMs.Value)
            .ToList();

        if (eligible.Count == 0)
            return Task.FromResult(Result<ModelSelectionResult>.Failure(
                "No model candidate satisfies the latency-based selection constraints"));

        var selected = eligible
            .OrderBy(c => ModelSelectionCalculator.EstimateLatency(c))
            .First();

        return Task.FromResult(Result<ModelSelectionResult>.Success(
            ModelSelectionCalculator.ToSelectionResult(selected, context, 1, "Selected model with lowest estimated latency")));
    }
}
