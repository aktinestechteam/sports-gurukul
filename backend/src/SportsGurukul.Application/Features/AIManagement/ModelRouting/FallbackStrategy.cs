namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class FallbackStrategy : IFallbackStrategy
{
    public Task<IReadOnlyList<Guid>> ResolveFallbackChainAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var chain = new List<Guid>();

        if (context.FallbackModelIds is { Count: > 0 })
        {
            chain.AddRange(context.FallbackModelIds
                .Where(id => candidates.Any(c => c.ModelId == id)));
        }

        chain.AddRange(candidates
            .Select(c => c.ModelId)
            .Where(id => !chain.Contains(id)));

        return Task.FromResult<IReadOnlyList<Guid>>(chain);
    }
}
