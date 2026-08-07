using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class XlsxTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Excel;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Excel;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        try
        {
            using var stream = new MemoryStream(content, writable: false);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            var workbookEntry = archive.GetEntry("xl/workbook.xml");
            var sheetsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels");
            if (workbookEntry == null || sheetsEntry == null)
            {
                return EmptyResultAsync();
            }

            var ns = XNamespace.None;
            var workbookXml = XDocument.Load(workbookEntry.Open());
            ns = workbookXml.Root?.Name.Namespace ?? XNamespace.None;
            var relationships = LoadRelationships(sheetsEntry);

            var builder = new StringBuilder();
            foreach (var sheet in workbookXml.Root?.Elements(ns + "sheets")?.Elements(ns + "sheet") ?? [])
            {
                var sheetName = sheet.Attribute("name")?.Value ?? "Sheet";
                var relationshipId = sheet.Attribute(ns + "id")?.Value;
                if (relationshipId == null || !relationships.TryGetValue(relationshipId, out var target))
                {
                    continue;
                }

                var sheetPart = archive.GetEntry("xl/" + target.TrimStart('/'));
                if (sheetPart == null)
                {
                    continue;
                }

                builder.Append("## ").Append(sheetName).AppendLine();
                var sheetXml = XDocument.Load(sheetPart.Open());
                var sheetNs = sheetXml.Root?.Name.Namespace ?? ns;
                foreach (var row in sheetXml.Descendants(sheetNs + "row"))
                {
                    var cells = row.Elements(sheetNs + "c")
                        .Select(c => c.Value.Trim())
                        .Where(v => v.Length > 0)
                        .ToList();
                    if (cells.Count > 0)
                    {
                        builder.Append("row: ").AppendJoin(" | ", cells).AppendLine();
                    }
                }
            }

            var text = builder.ToString();
            var sections = new[]
            {
                new DocumentSection("Document", 1, 0, text.Length) { Content = text }
            };
            return Task.FromResult(new ExtractedDocumentText(text, sections));
        }
        catch (InvalidDataException)
        {
            throw new InvalidDataException(
                $"'{document.FileName}' is not a valid Office Open XML Excel document. " +
                "Provide a .xlsx file or register a third-party extractor for legacy .xls support.");
        }
    }

    private static IReadOnlyDictionary<string, string> LoadRelationships(ZipArchiveEntry entry)
    {
        var result = new Dictionary<string, string>();
        using var stream = entry.Open();
        var xml = XDocument.Load(stream);
        foreach (var rel in xml.Root?.Elements() ?? [])
        {
            var id = rel.Attribute("Id")?.Value;
            var target = rel.Attribute("Target")?.Value;
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(target))
            {
                result[id] = target;
            }
        }
        return result;
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
