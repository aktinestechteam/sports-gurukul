using System.Text.RegularExpressions;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public partial class MarkdownParser : IDocumentParser
{
    [GeneratedRegex(@"^#{1,6}\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"!\[.*?\]\(.*?\)")]
    private static partial Regex ImageRegex();

    [GeneratedRegex(@"\[.*?\]\(.*?\)")]
    private static partial Regex LinkRegex();

    public DocumentFormat SupportedFormat => DocumentFormat.Markdown;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var text = System.Text.Encoding.UTF8.GetString(document.Content);

        var firstHeading = HeadingRegex().Match(text).Groups[1].Value;

        var plainText = StripMarkdown(text);

        var imageCount = ImageRegex().Matches(text).Count;

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "Markdown",
            ["ImageCount"] = imageCount.ToString(),
            ["HeadingCount"] = HeadingRegex().Matches(text).Count.ToString(),
            ["CharCount"] = text.Length.ToString()
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Markdown,
            Text: plainText,
            Title: string.IsNullOrEmpty(firstHeading) ? Path.GetFileNameWithoutExtension(document.FileName) : firstHeading,
            PageCount: null,
            DetectedLanguage: null,
            Author: null,
            CreatedDate: null,
            ModifiedDate: null,
            Metadata: metadata,
            Images: null,
            Status: ProcessingStatus.Extracted
        ));
    }

    private static string StripMarkdown(string markdown)
    {
        var text = ImageRegex().Replace(markdown, "");
        text = LinkRegex().Replace(text, m => m.Groups[1].Value);
        text = Regex.Replace(text, @"[*_~`#>{}\[\]\-+]", "");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        return text.Trim();
    }
}
