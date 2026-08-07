using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class ChunkingService : IChunkingService
{
    private readonly IChunkingStrategyRegistry _registry;

    public ChunkingService(IChunkingStrategyRegistry registry)
    {
        _registry = registry;
    }

    public IReadOnlyList<DocumentChunk> Chunk(
        KnowledgeDocument document,
        string text,
        ChunkingOptions? options = null,
        CancellationToken ct = default)
    {
        var resolved = options ?? new ChunkingOptions();
        var strategy = _registry.GetStrategy(resolved.Strategy);
        return strategy.Chunk(document, text, resolved, null, ct);
    }
}
