using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class FixedSizeChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.FixedSize;

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
        var chunkSize = Math.Max(1, options.ChunkSize);
        var overlap = Math.Min(Math.Max(0, options.ChunkOverlap), chunkSize / 2);
        var step = Math.Max(1, chunkSize - overlap);
        var offset = 0;
        var order = 0;

        while (offset < text.Length)
        {
            ct.ThrowIfCancellationRequested();
            var length = Math.Min(chunkSize, text.Length - offset);
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
