using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class SlidingWindowChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.SlidingWindow;

    public override IReadOnlyList<DocumentChunk> Chunk(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        IEmbeddingProvider? embeddingProvider = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<DocumentChunk>();
        }

        var chunks = new List<DocumentChunk>();
        var windowSize = Math.Max(1, options.ChunkSize);
        var overlap = Math.Min(Math.Max(1, options.ChunkOverlap), windowSize / 2);
        var step = Math.Max(1, windowSize - overlap);
        var offset = 0;
        var order = 0;

        while (offset < text.Length)
        {
            ct.ThrowIfCancellationRequested();
            var length = Math.Min(windowSize, text.Length - offset);
            var content = text.Substring(offset, length).Trim();

            if (content.Length >= options.MinChunkSize)
            {
                chunks.Add(CreateChunk(document, content, order, options));
                order++;
            }

            offset += step;
        }

        return chunks;
    }
}
