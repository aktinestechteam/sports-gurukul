using System.Text;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed class SemanticChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.Semantic;

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

        var sentences = SplitSentences(text);
        if (sentences.Count <= 1)
        {
            return new List<DocumentChunk>
            {
                CreateChunk(document, text, 0, options)
            };
        }

        if (embeddingProvider == null)
        {
            return FallbackToWindowed(document, sentences, options, ct);
        }

        var sentenceEmbeddings = EmbedSentences(document, sentences, embeddingProvider, options, ct);
        var groupBoundaries = FindGroupBoundaries(sentenceEmbeddings, options.SemanticMergeThreshold);
        var groups = GroupSentences(sentences, groupBoundaries);

        var chunks = new List<DocumentChunk>();
        var order = 0;
        foreach (var group in groups)
        {
            ct.ThrowIfCancellationRequested();
            var content = string.Join(" ", group);
            if (Measure(content, options) <= options.ChunkSize)
            {
                chunks.Add(CreateChunk(document, content, order, options));
                order++;
            }
            else
            {
                foreach (var piece in SplitGroup(document, group, options, order, ct))
                {
                    chunks.Add(piece);
                    order++;
                }
            }
        }

        return chunks;
    }

    private static IReadOnlyList<string> SplitSentences(string text)
    {
        var normalized = text.Replace("\r\n", " ").Replace('\n', ' ');
        var sentences = new List<string>();
        var start = 0;

        for (var i = 0; i < normalized.Length; i++)
        {
            var c = normalized[i];
            if (c is '.' or '!' or '?' || (c == ';' && i - start > 120))
            {
                var boundary = i + 1;
                while (boundary < normalized.Length && normalized[boundary] == ' ')
                {
                    boundary++;
                }

                var sentence = normalized[start..i].Trim();
                if (sentence.Length > 0)
                {
                    sentences.Add(sentence);
                }

                start = boundary;
                i = boundary - 1;
            }
        }

        var tail = normalized[start..].Trim();
        if (tail.Length > 0)
        {
            sentences.Add(tail);
        }

        return sentences;
    }

    private static IReadOnlyList<EmbeddingVector> EmbedSentences(
        KnowledgeDocument document,
        IReadOnlyList<string> sentences,
        IEmbeddingProvider provider,
        ChunkingOptions options,
        CancellationToken ct)
    {
        var results = new List<EmbeddingVector>(sentences.Count);
        var batchSize = Math.Max(8, Math.Min(64, sentences.Count));
        for (var i = 0; i < sentences.Count; i += batchSize)
        {
            ct.ThrowIfCancellationRequested();
            var batch = sentences.Skip(i).Take(batchSize).ToList();
            results.AddRange(provider.EmbedBatchAsync(batch, ct).GetAwaiter().GetResult());
        }

        return results;
    }

    private static IReadOnlyList<int> FindGroupBoundaries(
        IReadOnlyList<EmbeddingVector> embeddings,
        float mergeThreshold)
    {
        var boundaries = new List<int> { 0 };
        for (var i = 1; i < embeddings.Count; i++)
        {
            var similarity = CosineSimilarity(embeddings[i - 1], embeddings[i]);
            if (similarity < mergeThreshold)
            {
                boundaries.Add(i);
            }
        }

        boundaries.Add(embeddings.Count);
        return boundaries;
    }

    private static IReadOnlyList<IReadOnlyList<string>> GroupSentences(
        IReadOnlyList<string> sentences,
        IReadOnlyList<int> boundaries)
    {
        var groups = new List<IReadOnlyList<string>>();
        for (var i = 0; i < boundaries.Count - 1; i++)
        {
            var start = boundaries[i];
            var end = boundaries[i + 1];
            if (end > start)
            {
                groups.Add(sentences.Skip(start).Take(end - start).ToList());
            }
        }

        return groups;
    }

    private static float CosineSimilarity(EmbeddingVector a, EmbeddingVector b)
    {
        var length = Math.Min(a.Values.Length, b.Values.Length);
        if (length == 0)
        {
            return 0f;
        }

        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < length; i++)
        {
            dot += a.Values[i] * b.Values[i];
            normA += a.Values[i] * a.Values[i];
            normB += b.Values[i] * b.Values[i];
        }

        if (normA == 0 || normB == 0)
        {
            return 0f;
        }

        return (float)(dot / (Math.Sqrt(normA) * Math.Sqrt(normB)));
    }

    private static IReadOnlyList<DocumentChunk> FallbackToWindowed(
        KnowledgeDocument document,
        IReadOnlyList<string> sentences,
        ChunkingOptions options,
        CancellationToken ct)
    {
        var chunks = new List<DocumentChunk>();
        var buffer = new StringBuilder();
        var order = 0;

        foreach (var sentence in sentences)
        {
            ct.ThrowIfCancellationRequested();
            if (Measure(buffer.ToString(), options) + Measure(sentence, options) > options.ChunkSize
                && buffer.Length > 0)
            {
                chunks.Add(CreateChunk(document, buffer.ToString(), order, options));
                order++;
                buffer.Clear();
            }

            if (buffer.Length > 0)
            {
                buffer.Append(' ');
            }

            buffer.Append(sentence);
        }

        if (buffer.Length > 0)
        {
            chunks.Add(CreateChunk(document, buffer.ToString(), order, options));
        }

        return chunks;
    }

    private static IReadOnlyList<DocumentChunk> SplitGroup(
        KnowledgeDocument document,
        IReadOnlyList<string> sentences,
        ChunkingOptions options,
        int startOrder,
        CancellationToken ct)
    {
        var pieces = new List<DocumentChunk>();
        var buffer = new StringBuilder();
        var order = startOrder;

        foreach (var sentence in sentences)
        {
            if (Measure(buffer.ToString(), options) + Measure(sentence, options) > options.ChunkSize
                && buffer.Length > 0)
            {
                pieces.Add(CreateChunk(document, buffer.ToString(), order, options));
                order++;
                buffer.Clear();
            }

            if (buffer.Length > 0)
            {
                buffer.Append(' ');
            }

            buffer.Append(sentence);
        }

        if (buffer.Length > 0)
        {
            pieces.Add(CreateChunk(document, buffer.ToString(), order, options));
        }

        return pieces;
    }
}
