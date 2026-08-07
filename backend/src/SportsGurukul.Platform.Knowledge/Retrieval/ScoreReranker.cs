using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Retrieval;

internal sealed class ScoreReranker : IReranker
{
    public string Name => "score";

    public Task<IReadOnlyList<RetrievedChunk>> RerankAsync(
        string query,
        IReadOnlyList<RetrievedChunk> candidates,
        CancellationToken ct = default)
    {
        var ranked = candidates
            .OrderByDescending(c => c.Score)
            .ThenBy(c => c.Rank)
            .Select((c, i) => c with { Rank = i })
            .ToList();

        return Task.FromResult<IReadOnlyList<RetrievedChunk>>(ranked);
    }
}
