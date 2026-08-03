using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class CapabilityBasedModelSelectionStrategy : IModelSelectionStrategy
{
    public AIRoutingStrategy Strategy => AIRoutingStrategy.Accuracy;

    public Task<Result<ModelSelectionResult>> SelectAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var eligible = candidates
            .Where(c => ModelSelectionCalculator.MeetsCapabilities(c, context))
            .ToList();

        if (eligible.Count == 0)
            return Task.FromResult(Result<ModelSelectionResult>.Failure(
                "No model candidate satisfies the required capabilities"));

        var selected = eligible
            .OrderByDescending(c => ModelSelectionCalculator.CapabilityScore(c, context))
            .ThenByDescending(c => c.ContextWindow ?? 0)
            .First();

        return Task.FromResult(Result<ModelSelectionResult>.Success(
            ModelSelectionCalculator.ToSelectionResult(selected, context, 1, "Selected model that best matches required capabilities")));
    }
}
