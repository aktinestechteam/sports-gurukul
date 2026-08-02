using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public partial class CsvParser : IDocumentParser
{
    [GeneratedRegex("\"[^\"]*\"|[^,\n]+")]
    private static partial Regex CsvFieldRegex();

    public DocumentFormat SupportedFormat => DocumentFormat.Csv;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var content = System.Text.Encoding.UTF8.GetString(document.Content);
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();

        if (lines.Length > 0)
        {
            var headerFields = ParseCsvLine(lines[0]);
            var header = string.Join(" | ", headerFields);
            sb.AppendLine($"Columns: {header}");
            sb.AppendLine($"Row count: {lines.Length - 1}");
            sb.AppendLine();

            for (int i = 1; i < Math.Min(lines.Length, 21); i++)
            {
                var fields = ParseCsvLine(lines[i]);
                for (int j = 0; j < Math.Min(fields.Length, headerFields.Length); j++)
                {
                    sb.AppendLine($"{headerFields[j]}: {fields[j]}");
                }
                sb.AppendLine("---");
            }

            if (lines.Length > 21)
                sb.AppendLine($"... and {lines.Length - 21} more rows");
        }

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "CSV",
            ["RowCount"] = (lines.Length - 1).ToString(),
            ["ColumnCount"] = lines.Length > 0 ? ParseCsvLine(lines[0]).Length.ToString() : "0"
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Csv,
            Text: sb.ToString(),
            Title: Path.GetFileNameWithoutExtension(document.FileName),
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

    private static string[] ParseCsvLine(string line)
    {
        var matches = CsvFieldRegex().Matches(line);
        return matches.Select(m => m.Value.Trim('"')).ToArray();
    }
}
