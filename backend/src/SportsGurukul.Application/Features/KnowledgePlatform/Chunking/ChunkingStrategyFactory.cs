using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class ChunkingStrategyFactory : IChunkingStrategyFactory
{
    private readonly Dictionary<ChunkingStrategyType, IChunkingStrategy> _strategies;

    public ChunkingStrategyFactory(IEnumerable<IChunkingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.StrategyType);
    }

    public IChunkingStrategy GetStrategy(ChunkingStrategyType type) =>
        _strategies.TryGetValue(type, out var strategy) ? strategy
        : throw new NotSupportedException($"No chunking strategy registered for type: {type}");

    public bool SupportsStrategy(ChunkingStrategyType type) =>
        _strategies.ContainsKey(type);
}
