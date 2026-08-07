using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed partial class MarkdownTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Markdown;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Markdown;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var raw = DocumentContentReader.DecodeText(content, document.ContentType);
        var lines = raw.Replace("\r\n", "\n").Split('\n');
        var sections = new List<DocumentSection>();
        var heading = "Document";
        var level = 1;
        var startLine = 0;
        var contentStart = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            if (IsHeading(lines[i], out var nextLevel, out var nextHeading))
            {
                if (i > contentStart)
                {
                    sections.Add(BuildSection(heading, level, startLine, contentStart, i, lines));
                }

                heading = nextHeading;
                level = nextLevel;
                startLine = i;
                contentStart = i + 1;
            }
        }

        if (startLine < lines.Length)
        {
            sections.Add(BuildSection(heading, level, startLine, contentStart, lines.Length, lines));
        }

        if (sections.Count == 0)
        {
            var full = StripMarkdown(string.Join("\n", lines));
            sections.Add(new DocumentSection("Document", 1, 0, full.Length) { Content = full });
        }

        var text = string.Join("\n\n", sections.Select(s => s.Content));
        return Task.FromResult(new ExtractedDocumentText(text, sections));
    }

    private static bool IsHeading(string line, out int level, out string heading)
    {
        var trimmed = line.TrimEnd();
        if (trimmed.StartsWith("#") && trimmed.Length > 1)
        {
            level = trimmed.TakeWhile(c => c == '#').Count();
            heading = trimmed[level..].Trim().Trim('#').Trim();
            return heading.Length > 0;
        }

        level = 1;
        heading = string.Empty;
        return false;
    }

    private static DocumentSection BuildSection(
        string heading,
        int level,
        int startLine,
        int contentStart,
        int endLine,
        string[] lines)
    {
        var content = StripMarkdown(string.Join("\n", lines[contentStart..endLine]).Trim());
        return new DocumentSection(heading, level, startLine, endLine) { Content = content };
    }

    private static string StripMarkdown(string markdown)
    {
        var s = markdown;
        s = FencedCodeRegex().Replace(s, " ");
        s = InlineCodeRegex().Replace(s, m => m.Groups[1].Value);
        s = ImageRegex().Replace(s, " ");
        s = LinkRegex().Replace(s, "$1");
        s = BoldRegex().Replace(s, "$1");
        s = ItalicRegex().Replace(s, "$1");
        s = StrikethroughRegex().Replace(s, "$1");
        s = HeadingMarkRegex().Replace(s, string.Empty);
        s = HorizontalRuleRegex().Replace(s, string.Empty);
        return s.Trim();
    }

    [GeneratedRegex("```.*?```", RegexOptions.Singleline)]
    private static partial Regex FencedCodeRegex();

    [GeneratedRegex("`([^`]*)`")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"!\[[^\]]*\]\([^)]*\)")]
    private static partial Regex ImageRegex();

    [GeneratedRegex(@"\[([^\]]+)\]\([^)]*\)")]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"\*\*([^*]+)\*\*")]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"\*([^*]+)\*")]
    private static partial Regex ItalicRegex();

    [GeneratedRegex(@"~~([^~]+)~~")]
    private static partial Regex StrikethroughRegex();

    [GeneratedRegex(@"^\s{0,3}#+\s*", RegexOptions.Multiline)]
    private static partial Regex HeadingMarkRegex();

    [GeneratedRegex(@"^\s{0,3}(-{3,}|\*{3,}|_{3,})\s*$", RegexOptions.Multiline)]
    private static partial Regex HorizontalRuleRegex();
}
