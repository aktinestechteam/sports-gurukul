using System.Text;
using System.Text.Json;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class CitationService : ICitationService
{
    private readonly ICitationEngine? _engine;

    public CitationService(ICitationEngine? engine = null)
    {
        _engine = engine;
    }

    public Citation CreateCitation(SearchResult result) =>
        _engine?.GenerateCitation(result) ?? DefaultGenerateCitation(result);

    public List<Citation> CreateCitations(List<SearchResult> results) =>
        _engine?.GenerateCitations(results) ?? results.Select(DefaultGenerateCitation).ToList();

    public string ToMarkdown(List<Citation> citations) =>
        _engine?.FormatCitationsAsMarkdown(citations) ?? DefaultFormatMarkdown(citations);

    public string ToJson(List<Citation> citations) =>
        _engine?.FormatCitationsAsJson(citations) ?? DefaultFormatJson(citations);

    private static Citation DefaultGenerateCitation(SearchResult result)
    {
        var excerpt = result.Content.Length > 200
            ? result.Content[..200] + "..."
            : result.Content;

        return new Citation(
            DocumentName: result.DocumentName,
            Section: result.Section,
            PageNumber: result.PageNumber,
            ChunkId: result.ChunkId,
            Confidence: result.Score,
            SourceLink: result.Citation.SourceLink,
            Excerpt: excerpt
        );
    }

    private static string DefaultFormatMarkdown(List<Citation> citations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## References");
        sb.AppendLine();

        for (int i = 0; i < citations.Count; i++)
        {
            var c = citations[i];
            sb.AppendLine($"[^{i + 1}]: **{c.DocumentName}**");
            if (c.Section != null) sb.AppendLine($"      Section: {c.Section}");
            if (c.PageNumber.HasValue) sb.AppendLine($"      Page: {c.PageNumber}");
            if (c.SourceLink != null) sb.AppendLine($"      Source: {c.SourceLink}");
            sb.AppendLine($"      Confidence: {c.Confidence:P1}");
            sb.AppendLine($"      > {c.Excerpt}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string DefaultFormatJson(List<Citation> citations)
    {
        return JsonSerializer.Serialize(new { citations }, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
