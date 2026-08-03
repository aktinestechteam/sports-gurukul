using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Retrieval;

internal sealed class RrfReranker : IReranker
{
    public string Name => "rrf";

    private const int RrfConstant = 60;

    public Task<IReadOnlyList<RetrievedChunk>> RerankAsync(
        string query,
        IReadOnlyList<RetrievedChunk> candidates,
        CancellationToken ct = default)
    {
        var byChunk = candidates.GroupBy(c => c.Chunk.Id).ToList();
        var scored = new List<(RetrievedChunk Candidate, double Score)>();

        foreach (var group in byChunk)
        {
            var score = group.Sum(c => 1.0 / (RrfConstant + c.Rank + 1));
            scored.Add((group.First(), score));
        }

        var ranked = scored
            .OrderByDescending(s => s.Score)
            .Select((s, i) => s.Candidate with { Score = (float)s.Score, Rank = i })
            .ToList();

        return Task.FromResult<IReadOnlyList<RetrievedChunk>>(ranked);
    }
}
