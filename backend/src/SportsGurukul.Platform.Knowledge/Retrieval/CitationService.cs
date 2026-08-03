using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Retrieval;

internal sealed class CitationService : ICitationService
{
    public IReadOnlyList<Citation> BuildCitations(IReadOnlyList<RetrievedChunk> chunks)
    {
        var citations = new List<Citation>(chunks.Count);
        foreach (var chunk in chunks)
        {
            citations.Add(new Citation(
                chunk.DocumentName ?? "Untitled document",
                chunk.Chunk.Section ?? chunk.Chunk.Heading,
                chunk.Chunk.PageNumber,
                chunk.Chunk.Id,
                Math.Clamp(chunk.Score, 0f, 1f),
                chunk.SourceLink,
                chunk.DocumentId,
                chunk.Chunk.Order));
        }

        return citations;
    }
}
