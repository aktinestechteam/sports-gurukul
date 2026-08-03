using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class CostBasedModelSelectionStrategy : IModelSelectionStrategy
{
    public AIRoutingStrategy Strategy => AIRoutingStrategy.Cost;

    public Task<Result<ModelSelectionResult>> SelectAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var eligible = candidates
            .Where(c => ModelSelectionCalculator.MeetsCapabilities(c, context))
            .Where(c => context.MaxCostPerRequest is null
                || (ModelSelectionCalculator.EstimateCost(c, context) ?? 0m) <= context.MaxCostPerRequest.Value)
            .ToList();

        if (eligible.Count == 0)
            return Task.FromResult(Result<ModelSelectionResult>.Failure(
                "No model candidate satisfies the cost-based selection constraints"));

        var selected = eligible
            .OrderBy(c => ModelSelectionCalculator.EstimateCost(c, context))
            .First();

        return Task.FromResult(Result<ModelSelectionResult>.Success(
            ModelSelectionCalculator.ToSelectionResult(selected, context, 1, "Selected model with lowest estimated cost")));
    }
}
