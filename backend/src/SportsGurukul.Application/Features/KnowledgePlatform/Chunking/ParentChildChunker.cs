using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class ParentChildChunker : IChunkingStrategy
{
    public ChunkingStrategyType StrategyType => ChunkingStrategyType.ParentChild;

    public Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var documentId = document.Id;
        var chunks = new List<DocumentChunk>();

        var parentMaxSize = options.ParentChunkSize ?? 2048;
        var childMaxSize = options.ChildChunkSize ?? 256;
        var overlap = options.ChunkOverlap;

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var parentIndex = 0;

        while (parentIndex < words.Length)
        {
            var parentWords = words.Skip(parentIndex).Take(parentMaxSize).ToArray();
            if (parentWords.Length == 0) break;

            var parentContent = string.Join(' ', parentWords);

            var parentId = $"{documentId}_parent_{chunks.Count(p => p.ParentChunkId == null)}";
            var parentChunkIndex = chunks.Count;

            var parentChunk = new DocumentChunk(
                Id: parentId,
                DocumentId: documentId,
                ChunkIndex: parentChunkIndex,
                Content: parentContent,
                TokenCount: parentWords.Length,
                CharacterCount: parentContent.Length,
                Heading: null,
                PageNumber: null,
                ParentChunkId: null,
                Metadata: new Dictionary<string, string>
                {
                    ["strategy"] = "parent_child",
                    ["level"] = "parent",
                    ["parent_size"] = parentMaxSize.ToString()
                },
                Strategy: ChunkingStrategyType.ParentChild
            );
            chunks.Add(parentChunk);

            var childIndex = 0;
            var childStart = 0;
            while (childStart < parentWords.Length)
            {
                var childWords = parentWords.Skip(childStart).Take(childMaxSize).ToArray();
                if (childWords.Length == 0) break;

                var childContent = string.Join(' ', childWords);

                chunks.Add(new DocumentChunk(
                    Id: $"{parentId}_child_{childIndex}",
                    DocumentId: documentId,
                    ChunkIndex: parentChunkIndex + childIndex + 1,
                    Content: childContent,
                    TokenCount: childWords.Length,
                    CharacterCount: childContent.Length,
                    Heading: null,
                    PageNumber: null,
                    ParentChunkId: parentChunkIndex,
                    Metadata: new Dictionary<string, string>
                    {
                        ["strategy"] = "parent_child",
                        ["level"] = "child",
                        ["parent_id"] = parentId,
                        ["child_size"] = childMaxSize.ToString()
                    },
                    Strategy: ChunkingStrategyType.ParentChild
                ));

                childIndex++;
                childStart += childMaxSize - overlap;
            }

            parentIndex += parentMaxSize - overlap;
        }

        return Task.FromResult(chunks);
    }
}
