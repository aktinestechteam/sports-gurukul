using System.Text.RegularExpressions;
using System.Web;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public partial class HtmlParser : IDocumentParser
{
    [GeneratedRegex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleRegex();

    [GeneratedRegex(@"<meta[^>]+name=[""']description[""'][^>]+content=[""']([^""']*)[""'][^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex MetaDescriptionRegex();

    [GeneratedRegex(@"<[^>]+>", RegexOptions.IgnoreCase)]
    private static partial Regex HtmlTagRegex();

    public DocumentFormat SupportedFormat => DocumentFormat.Html;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var html = System.Text.Encoding.UTF8.GetString(document.Content);

        var title = TitleRegex().Match(html).Groups[1].Value;
        var description = MetaDescriptionRegex().Match(html).Groups[1].Value;
        var text = HttpUtility.HtmlDecode(HtmlTagRegex().Replace(html, " "));

        text = Regex.Replace(text, @"\s+", " ").Trim();

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "HTML",
            ["CharCount"] = html.Length.ToString(),
            ["TextLength"] = text.Length.ToString()
        };

        if (!string.IsNullOrEmpty(description))
            metadata["Description"] = description;

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Html,
            Text: text,
            Title: string.IsNullOrEmpty(title) ? Path.GetFileNameWithoutExtension(document.FileName) : title,
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
}
