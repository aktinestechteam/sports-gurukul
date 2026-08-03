using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public interface IFallbackStrategy
{
    Task<IReadOnlyList<Guid>> ResolveFallbackChainAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default);
}
