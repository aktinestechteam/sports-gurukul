using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class PptxTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.PowerPoint;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.PowerPoint;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        try
        {
            using var stream = new MemoryStream(content, writable: false);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            var slides = archive.Entries
                .Where(e => e.FullName.StartsWith("ppt/slides/slide", StringComparison.OrdinalIgnoreCase)
                            && e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.FullName, StringComparer.Ordinal)
                .ToList();

            if (slides.Count == 0)
            {
                return EmptyResultAsync();
            }

            var ns = XNamespace.None;
            var sections = new List<DocumentSection>();
            var offset = 0;

            foreach (var slide in slides)
            {
                var xml = XDocument.Load(slide.Open());
                ns = xml.Root?.Name.Namespace ?? XNamespace.None;

                var texts = xml.Descendants(ns + "t").Select(t => t.Value.Trim()).Where(v => v.Length > 0).ToList();
                var body = string.Join("\n", texts);

                var page = ParseSlideNumber(slide.FullName);
                var heading = texts.FirstOrDefault() ?? $"Slide {page}";
                sections.Add(new DocumentSection(heading, 1, offset, offset + body.Length, page)
                {
                    Content = body
                });
                offset += body.Length + 1;
            }

            var text = string.Join("\n\n", sections.Select(s => s.Content));
            return Task.FromResult(new ExtractedDocumentText(text, sections));
        }
        catch (InvalidDataException)
        {
            throw new InvalidDataException(
                $"'{document.FileName}' is not a valid Office Open XML PowerPoint document. " +
                "Provide a .pptx file or register a third-party extractor for legacy .ppt support.");
        }
    }

    private static int ParseSlideNumber(string fullName)
    {
        var name = Path.GetFileNameWithoutExtension(fullName);
        return int.TryParse(name.AsSpan("slide".Length), out var n) ? n : 0;
    }

    private static Task<ExtractedDocumentText> EmptyResultAsync()
    {
        var sections = new[]
        {
            new DocumentSection("Document", 1, 0, 0) { Content = string.Empty }
        };
        return Task.FromResult(new ExtractedDocumentText(string.Empty, sections));
    }
}
