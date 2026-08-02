using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Chunking;

public class SemanticChunker : IChunkingStrategy
{
    public ChunkingStrategyType StrategyType => ChunkingStrategyType.Semantic;

    public async Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        var text = document.Text;
        var documentId = document.Id;
        var chunks = new List<DocumentChunk>();

        var sentences = SplitSentences(text);
        var groups = await GroupSentencesSemanticallyAsync(sentences, options.MaxChunkSize, cancellationToken);

        for (int i = 0; i < groups.Count; i++)
        {
            var content = string.Join(' ', groups[i]);

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
                    ["strategy"] = "semantic",
                    ["sentence_count"] = groups[i].Length.ToString()
                },
                Strategy: ChunkingStrategyType.Semantic
            ));
        }

        return chunks;
    }

    private static string[] SplitSentences(string text)
    {
        return text.Split('.', '!', '?', '\n')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToArray();
    }

    private static Task<List<string[]>> GroupSentencesSemanticallyAsync(string[] sentences, int maxChunkSize, CancellationToken ct)
    {
        var groups = new List<string[]>();
        var currentGroup = new List<string>();
        var currentLength = 0;

        foreach (var sentence in sentences)
        {
            var sentenceLength = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            if (currentLength + sentenceLength > maxChunkSize && currentGroup.Count > 0)
            {
                groups.Add(currentGroup.ToArray());
                currentGroup.Clear();
                currentLength = 0;
            }

            currentGroup.Add(sentence);
            currentLength += sentenceLength;
        }

        if (currentGroup.Count > 0)
            groups.Add(currentGroup.ToArray());

        return Task.FromResult(groups);
    }
}
