using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class ParentChildChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.ParentChild;

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

        var parentOptions = options with
        {
            Strategy = ChunkingStrategyType.Recursive,
            ChunkSize = options.ParentChunkSize,
            ChunkOverlap = 0
        };

        var parentChunks = SplitFixed(document, text, parentOptions, options.ParentChunkSize, ct);
        var chunks = new List<DocumentChunk>();
        var order = 0;

        foreach (var parent in parentChunks)
        {
            ct.ThrowIfCancellationRequested();
            chunks.Add(parent);
            var parentOrder = order;
            order++;

            if (options.ChildChunkSize >= options.ParentChunkSize)
            {
                continue;
            }

            var childOptions = options with
            {
                Strategy = ChunkingStrategyType.Recursive,
                ChunkSize = options.ChildChunkSize,
                ChunkOverlap = Math.Min(options.ChunkOverlap, options.ChildChunkSize / 4)
            };

            foreach (var child in SplitFixed(document, parent.Text, childOptions, options.ChildChunkSize, ct))
            {
                chunks.Add(child with { ParentChunkId = parentOrder });
                order++;
            }
        }

        return chunks;
    }

    private static IReadOnlyList<DocumentChunk> SplitFixed(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        int chunkSize,
        CancellationToken ct)
    {
        var chunks = new List<DocumentChunk>();
        var offset = 0;
        var order = 0;
        chunkSize = Math.Max(1, chunkSize);

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

            offset += chunkSize;
        }

        return chunks;
    }
}
