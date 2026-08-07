using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class BalancedModelSelectionStrategy : IModelSelectionStrategy
{
    public AIRoutingStrategy Strategy => AIRoutingStrategy.Balanced;

    public Task<Result<ModelSelectionResult>> SelectAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var eligible = candidates
            .Where(c => ModelSelectionCalculator.MeetsCapabilities(c, context))
            .Where(c => context.MaxCostPerRequest is null
                || (ModelSelectionCalculator.EstimateCost(c, context) ?? 0m) <= context.MaxCostPerRequest.Value)
            .Where(c => context.MaxLatencyMs is null
                || (ModelSelectionCalculator.EstimateLatency(c) ?? int.MaxValue) <= context.MaxLatencyMs.Value)
            .ToList();

        if (eligible.Count == 0)
            return Task.FromResult(Result<ModelSelectionResult>.Failure(
                "No model candidate satisfies the balanced selection constraints"));

        var selected = eligible
            .OrderByDescending(c => ModelSelectionCalculator.BalancedScore(c, context))
            .First();

        return Task.FromResult(Result<ModelSelectionResult>.Success(
            ModelSelectionCalculator.ToSelectionResult(selected, context, 1, "Selected model with highest balanced score")));
    }
}
