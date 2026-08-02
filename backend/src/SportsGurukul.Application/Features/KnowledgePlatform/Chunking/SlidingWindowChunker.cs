using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class SlidingWindowChunker : IChunkingStrategy
{
    public ChunkingStrategyType StrategyType => ChunkingStrategyType.SlidingWindow;

    public Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var windowSize = options.MaxChunkSize;
        var stride = windowSize - options.ChunkOverlap;
        var documentId = document.Id;
        var chunks = new List<DocumentChunk>();

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var index = 0;

        while (index < words.Length)
        {
            var windowWords = words.Skip(index).Take(windowSize).ToArray();
            if (windowWords.Length < options.MinChunkSize) break;

            var content = string.Join(' ', windowWords);

            chunks.Add(new DocumentChunk(
                Id: $"{documentId}_chunk_{chunks.Count}",
                DocumentId: documentId,
                ChunkIndex: chunks.Count,
                Content: content,
                TokenCount: windowWords.Length,
                CharacterCount: content.Length,
                Heading: null,
                PageNumber: null,
                ParentChunkId: null,
                Metadata: new Dictionary<string, string>
                {
                    ["strategy"] = "sliding_window",
                    ["window_start"] = index.ToString(),
                    ["window_end"] = (index + windowWords.Length - 1).ToString()
                },
                Strategy: ChunkingStrategyType.SlidingWindow
            ));

            index += stride;
        }

        return Task.FromResult(chunks);
    }
}
