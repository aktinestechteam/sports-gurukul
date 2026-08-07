using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class DocxTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Word;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Word;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var paragraphs = new List<(string Style, string Text)>();
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var stream = new MemoryStream(content, writable: false);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            var documentPart = archive.GetEntry("word/document.xml");
            if (documentPart == null)
            {
                return FallbackAsync();
            }

            var docXml = XDocument.Load(documentPart.Open());
            var ns = docXml.Root?.Name.Namespace ?? XNamespace.None;
            var paragraphsXml = docXml.Descendants(ns + "p");

            foreach (var p in paragraphsXml)
            {
                var style = p.Descendants(ns + "pStyle")
                    .FirstOrDefault()?
                    .Attribute(ns + "val")?.Value ?? string.Empty;
                var text = string.Concat(p
                    .Descendants(ns + "t")
                    .Select(t => t.Value));
                if (!string.IsNullOrWhiteSpace(text))
                {
                    paragraphs.Add((style, text.Trim()));
                }
            }

            foreach (var meta in new[] { "core.xml", "custom.xml" })
            {
                var entry = archive.GetEntry($"docProps/{meta}");
                if (entry == null)
                {
                    continue;
                }

                var xml = XDocument.Load(entry.Open());
                foreach (var element in xml.Descendants())
                {
                    if (element.HasElements)
                    {
                        continue;
                    }

                    var value = element.Value?.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        metadata[element.Name.LocalName] = value;
                    }
                }
            }
        }
        catch (InvalidDataException)
        {
            return UnsupportedFallbackAsync(document);
        }

        return BuildResultAsync(paragraphs, metadata);
    }

    private static Task<ExtractedDocumentText> BuildResultAsync(
        IReadOnlyList<(string Style, string Text)> paragraphs,
        IReadOnlyDictionary<string, string> metadata)
    {
        var sections = new List<DocumentSection>();
        var current = new List<string>();
        var currentHeading = "Document";
        var currentLevel = 1;
        var offset = 0;

        foreach (var (style, text) in paragraphs)
        {
            var isHeading = style.StartsWith("Heading", StringComparison.OrdinalIgnoreCase)
                            || (style.Length == 1 && char.IsDigit(style[0]));

            if (isHeading)
            {
                FlushSection(sections, currentHeading, currentLevel, current, ref offset);
                currentHeading = text;
                currentLevel = int.TryParse(style.AsSpan(style.Length - 1), out var level) ? level : 1;
                current = new List<string>();
            }
            else
            {
                current.Add(text);
            }
        }

        FlushSection(sections, currentHeading, currentLevel, current, ref offset);

        var body = string.Join("\n\n", sections.Select(s => s.Content));
        return Task.FromResult(new ExtractedDocumentText(body, sections, metadata));
    }

    private static void FlushSection(
        ICollection<DocumentSection> sections,
        string heading,
        int level,
        IReadOnlyList<string> paragraphs,
        ref int offset)
    {
        var content = string.Join("\n\n", paragraphs);
        sections.Add(new DocumentSection(heading, level, offset, offset + Math.Max(content.Length, heading.Length))
        {
            Content = content.Length == 0 ? heading : content
        });
        offset += content.Length + 1;
    }

    private static Task<ExtractedDocumentText> FallbackAsync()
    {
        var content = string.Empty;
        var sections = new[]
        {
            new DocumentSection("Document", 1, 0, content.Length) { Content = content }
        };
        return Task.FromResult(new ExtractedDocumentText(content, sections));
    }

    private static Task<ExtractedDocumentText> UnsupportedFallbackAsync(KnowledgeDocument document) =>
        throw new InvalidDataException(
            $"'{document.FileName}' is not a valid Office Open XML Word document. " +
            "Provide a .docx file or register a third-party extractor (e.g. DocumentFormat.OpenXml) for legacy .doc support.");
}
