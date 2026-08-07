using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class RecursiveChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.Recursive;

    public override IReadOnlyList<DocumentChunk> Chunk(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        IEmbeddingProvider? embeddingProvider = null,
        CancellationToken ct = default)
    {
        var chunks = new List<DocumentChunk>();
        var separators = options.Separators.Count > 0
            ? options.Separators
            : new[] { "\n\n", "\n", ". ", "! ", "? ", "; ", ", ", " ", "" };
        SplitRecursively(document, text, options, separators, 0, chunks, ref ct, parent: null);
        return chunks;
    }

    private static void SplitRecursively(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        IReadOnlyList<string> separators,
        int separatorIndex,
        ICollection<DocumentChunk> chunks,
        ref CancellationToken ct,
        int? parent)
    {
        ct.ThrowIfCancellationRequested();
        var trimmed = text.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        var size = Measure(trimmed, options);
        if (size <= options.ChunkSize)
        {
            if (size >= options.MinChunkSize)
            {
                chunks.Add(CreateChunk(document, trimmed, chunks.Count, options, parentChunkId: parent));
            }

            return;
        }

        var separator = separators[Math.Min(separatorIndex, separators.Count - 1)];
        var pieces = SplitOn(trimmed, separator);
        var (left, right) = Balance(pieces, options.ChunkSize);

        if (left.Count == 0 || right.Count == 0)
        {
            if (separatorIndex + 1 >= separators.Count)
            {
                SplitByHardLimit(document, trimmed, options, chunks, ref ct);
                return;
            }

            SplitRecursively(document, trimmed, options, separators, separatorIndex + 1, chunks, ref ct, parent);
            return;
        }

        SplitRecursively(document, string.Join(separator, left), options, separators, separatorIndex, chunks, ref ct, parent);
        SplitRecursively(document, string.Join(separator, right), options, separators, separatorIndex, chunks, ref ct, parent);
    }

    private static IReadOnlyList<string> SplitOn(string text, string separator)
    {
        if (separator.Length == 0)
        {
            var result = new List<string>(text.Length);
            foreach (var c in text)
            {
                result.Add(c.ToString());
            }

            return result;
        }

        return text.Split(separator, StringSplitOptions.None);
    }

    private static (IReadOnlyList<string> Left, IReadOnlyList<string> Right) Balance(
        IReadOnlyList<string> pieces,
        int chunkSize)
    {
        var cumulative = 0;
        var splitIndex = 0;
        for (var i = 0; i < pieces.Count; i++)
        {
            cumulative += pieces[i].Length + (i > 0 ? 1 : 0);
            if (cumulative >= chunkSize && i > 0)
            {
                splitIndex = i;
                break;
            }

            splitIndex = i + 1;
        }

        if (splitIndex == 0 || splitIndex >= pieces.Count)
        {
            return (Array.Empty<string>(), pieces);
        }

        return (pieces.Take(splitIndex).ToList(), pieces.Skip(splitIndex).ToList());
    }

    private static void SplitByHardLimit(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        ICollection<DocumentChunk> chunks,
        ref CancellationToken ct)
    {
        var chunkSize = Math.Max(1, options.ChunkSize);
        var offset = 0;
        while (offset < text.Length)
        {
            ct.ThrowIfCancellationRequested();
            var length = Math.Min(chunkSize, text.Length - offset);
            var content = text.Substring(offset, length);
            if (content.Length >= options.MinChunkSize)
            {
                chunks.Add(CreateChunk(document, content, chunks.Count, options));
            }

            offset += chunkSize;
        }
    }
}
