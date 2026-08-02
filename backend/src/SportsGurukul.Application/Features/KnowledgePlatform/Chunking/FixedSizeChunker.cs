using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class FixedSizeChunker : IChunkingStrategy
{
    public ChunkingStrategyType StrategyType => ChunkingStrategyType.FixedSize;

    public Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var maxSize = options.MaxChunkSize;
        var overlap = options.ChunkOverlap;
        var chunks = new List<DocumentChunk>();

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var documentId = document.Id;

        for (int i = 0; i < words.Length; i += maxSize - overlap)
        {
            var chunkWords = words.Skip(i).Take(maxSize).ToArray();
            if (chunkWords.Length == 0) break;

            var content = string.Join(' ', chunkWords);
            var charCount = content.Length;

            chunks.Add(new DocumentChunk(
                Id: $"{documentId}_chunk_{chunks.Count}",
                DocumentId: documentId,
                ChunkIndex: chunks.Count,
                Content: content,
                TokenCount: chunkWords.Length,
                CharacterCount: charCount,
                Heading: null,
                PageNumber: null,
                ParentChunkId: null,
                Metadata: new Dictionary<string, string>
                {
                    ["strategy"] = "fixed_size",
                    ["max_chunk_size"] = maxSize.ToString(),
                    ["overlap"] = overlap.ToString()
                },
                Strategy: ChunkingStrategyType.FixedSize
            ));
        }

        return Task.FromResult(chunks);
    }
}
