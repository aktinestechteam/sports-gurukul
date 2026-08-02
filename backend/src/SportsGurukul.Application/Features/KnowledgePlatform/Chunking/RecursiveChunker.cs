using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class RecursiveChunker : IChunkingStrategy
{
    public ChunkingStrategyType StrategyType => ChunkingStrategyType.Recursive;

    public Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var documentId = document.Id;
        var chunks = new List<DocumentChunk>();

        var separators = new[] { "\n\n\n\n\n\n\n\n", "\n\n\n\n\n\n\n", "\n\n\n\n\n\n", "\n\n\n\n\n", "\n\n\n\n", "\n\n\n", "\n\n", "\n", ". ", " ", "" };

        var result = RecursiveSplit(text, options.MaxChunkSize, options.ChunkOverlap, separators);

        for (int i = 0; i < result.Count; i++)
        {
            var content = result[i];
            chunks.Add(new DocumentChunk(
                Id: $"{documentId}_chunk_{i}",
                DocumentId: documentId,
                ChunkIndex: i,
                Content: content,
                TokenCount: content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                CharacterCount: content.Length,
                Heading: null,
                PageNumber: null,
                ParentChunkId: null,
                Metadata: new Dictionary<string, string>
                {
                    ["strategy"] = "recursive"
                },
                Strategy: ChunkingStrategyType.Recursive
            ));
        }

        return Task.FromResult(chunks);
    }

    private static List<string> RecursiveSplit(string text, int maxSize, int overlap, string[] separators, int depth = 0)
    {
        if (depth > separators.Length || text.Length <= maxSize)
            return new List<string> { text };

        var separator = separators[depth];
        var parts = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();
        var current = new List<string>();
        var currentLength = 0;

        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (currentLength + trimmed.Length + separator.Length > maxSize && current.Count > 0)
            {
                result.Add(string.Join(separator, current));
                var overlapCount = Math.Min(current.Count, Math.Max(1, overlap / 10));
                current = current.Skip(current.Count - overlapCount).ToList();
                currentLength = current.Sum(c => c.Length + separator.Length);
            }

            current.Add(trimmed);
            currentLength += trimmed.Length + separator.Length;
        }

        if (current.Count > 0)
            result.Add(string.Join(separator, current));

        var finalResult = new List<string>();
        foreach (var chunk in result)
        {
            if (chunk.Length > maxSize && depth < separators.Length)
                finalResult.AddRange(RecursiveSplit(chunk, maxSize, overlap, separators, depth + 1));
            else
                finalResult.Add(chunk);
        }

        return finalResult;
    }
}
