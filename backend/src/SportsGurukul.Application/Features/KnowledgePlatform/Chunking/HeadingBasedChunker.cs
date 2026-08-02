using System.Text.RegularExpressions;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public partial class HeadingBasedChunker : IChunkingStrategy
{
    [GeneratedRegex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex HeadingRegex();

    public ChunkingStrategyType StrategyType => ChunkingStrategyType.HeadingBased;

    public Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var documentId = document.Id;
        var chunks = new List<DocumentChunk>();

        var lines = text.Split('\n');
        var currentHeading = "Introduction";
        var currentContent = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var match = HeadingRegex().Match(line);

            if (match.Success)
            {
                if (currentContent.Count > 0)
                {
                    chunks.Add(CreateChunk(documentId, chunks.Count, currentHeading, currentContent));
                    currentContent.Clear();
                }

                currentHeading = match.Groups[2].Value.Trim();
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                currentContent.Add(line);
            }
        }

        if (currentContent.Count > 0)
            chunks.Add(CreateChunk(documentId, chunks.Count, currentHeading, currentContent));

        return Task.FromResult(chunks);
    }

    private static DocumentChunk CreateChunk(string documentId, int index, string heading, List<string> content)
    {
        var text = string.Join(' ', content);

        return new DocumentChunk(
            Id: $"{documentId}_chunk_{index}",
            DocumentId: documentId,
            ChunkIndex: index,
            Content: text,
            TokenCount: text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            CharacterCount: text.Length,
            Heading: heading,
            PageNumber: null,
            ParentChunkId: null,
            Metadata: new Dictionary<string, string>
            {
                ["strategy"] = "heading_based",
                ["heading"] = heading
            },
            Strategy: ChunkingStrategyType.HeadingBased
        );
    }
}
