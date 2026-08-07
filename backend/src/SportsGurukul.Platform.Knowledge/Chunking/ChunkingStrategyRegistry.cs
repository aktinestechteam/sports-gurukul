using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

public interface IChunkingStrategyRegistry
{
    IChunkingStrategy GetStrategy(ChunkingStrategyType type);
    void Register(IChunkingStrategy strategy);
}

internal sealed class ChunkingStrategyRegistry : IChunkingStrategyRegistry
{
    private readonly Dictionary<ChunkingStrategyType, IChunkingStrategy> _strategies;
    private readonly object _sync = new();

    public ChunkingStrategyRegistry()
        : this(new IChunkingStrategy[]
        {
            new RecursiveChunker(),
            new FixedSizeChunker(),
            new SlidingWindowChunker(),
            new HeadingBasedChunker(),
            new SemanticChunker(),
            new ParentChildChunker()
        })
    {
    }

    internal ChunkingStrategyRegistry(IEnumerable<IChunkingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Type, s => s);
    }

    public IChunkingStrategy GetStrategy(ChunkingStrategyType type)
    {
        lock (_sync)
        {
            return _strategies.TryGetValue(type, out var strategy)
                ? strategy
                : throw new InvalidOperationException($"No chunking strategy registered for '{type}'.");
        }
    }

    public void Register(IChunkingStrategy strategy)
    {
        lock (_sync)
        {
            _strategies[strategy.Type] = strategy;
        }
    }
}
