using System.Text;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class CsvTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Csv;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Csv;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var raw = DocumentContentReader.DecodeText(content, document.ContentType);
        var builder = new StringBuilder();
        var rowIndex = 0;

        foreach (var line in raw.Replace("\r\n", "\n").Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var cells = ParseCsvLine(line);
            builder.Append("row ").Append(rowIndex).Append(": ")
                .AppendJoin(" | ", cells).Append('\n');
            rowIndex++;
        }

        var text = builder.ToString();
        var sections = new[]
        {
            new DocumentSection("Document", 1, 0, text.Length) { Content = text }
        };

        return Task.FromResult(new ExtractedDocumentText(text, sections));
    }

    private static IReadOnlyList<string> ParseCsvLine(string line)
    {
        var cells = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                cells.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        cells.Add(current.ToString().Trim());
        return cells;
    }
}
