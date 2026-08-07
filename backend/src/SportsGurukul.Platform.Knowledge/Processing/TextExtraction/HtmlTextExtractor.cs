using System.Net;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed partial class HtmlTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Html;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Html;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var html = DocumentContentReader.DecodeText(content, document.ContentType);
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var title = TitleRegex().Match(html);
        if (title.Success)
        {
            metadata["title"] = WebUtility.HtmlDecode(title.Groups[1].Value.Trim());
        }

        var description = Regex.Match(html, "<meta[^>]*name=\"description\"[^>]*content=\"([^\"]*)\"", RegexOptions.IgnoreCase);
        if (description.Success)
        {
            metadata["description"] = WebUtility.HtmlDecode(description.Groups[1].Value.Trim());
        }

        var text = StripTags(html);
        var sections = new List<DocumentSection>();
        foreach (Match match in HeadingRegex().Matches(html))
        {
            var level = int.Parse(match.Groups[1].Value[1..]);
            var headingText = StripTags(match.Groups[2].Value).Trim();
            sections.Add(new DocumentSection(headingText, level, 0, 0) { Content = headingText });
        }

        if (sections.Count == 0)
        {
            sections.Add(new DocumentSection("Document", 1, 0, text.Length) { Content = text });
        }

        return Task.FromResult(new ExtractedDocumentText(text, sections, metadata));
    }

    private static string StripTags(string html)
    {
        var noScript = ScriptRegex().Replace(html, " ");
        noScript = StyleRegex().Replace(noScript, " ");
        var text = TagRegex().Replace(noScript, " ");
        return WebUtility.HtmlDecode(text).Trim();
    }

    [GeneratedRegex("<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleRegex();

    [GeneratedRegex(@"<(h[1-6])[^>]*>(.*?)</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HeadingRegex();

    [GeneratedRegex("<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ScriptRegex();

    [GeneratedRegex("<style[^>]*>.*?</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StyleRegex();

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex TagRegex();
}
