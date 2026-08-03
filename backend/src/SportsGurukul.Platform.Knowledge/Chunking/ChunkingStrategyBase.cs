using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal abstract class ChunkingStrategyBase : IChunkingStrategy
{
    public abstract ChunkingStrategyType Type { get; }

    public abstract IReadOnlyList<DocumentChunk> Chunk(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        IEmbeddingProvider? embeddingProvider = null,
        CancellationToken ct = default);

    protected static int Measure(string text, ChunkingOptions options)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        if (options.UseTokenEstimation)
        {
            var words = 0;
            var inWord = false;
            foreach (var c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    inWord = false;
                }
                else if (!inWord)
                {
                    words++;
                    inWord = true;
                }
            }

            return Math.Max(1, (int)Math.Ceiling(words * 1.33));
        }

        return text.Length;
    }

    protected static DocumentChunk CreateChunk(
        KnowledgeDocument document,
        string text,
        int order,
        ChunkingOptions options,
        int? pageNumber = null,
        string? section = null,
        string? heading = null,
            int? parentChunkId = null)
    {
        var clean = text?.Trim() ?? string.Empty;
        return new DocumentChunk(
            Guid.NewGuid(),
            document.Id,
            document.IndexName ?? "default",
            clean,
            order,
            pageNumber,
            section,
            heading,
            parentChunkId,
            Measure(clean, options));
    }
}
