using System.Text;
using System.Text.Json;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class JsonParser : IDocumentParser
{
    public DocumentFormat SupportedFormat => DocumentFormat.Json;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var content = System.Text.Encoding.UTF8.GetString(document.Content);

        var prettyJson = FormatJson(content);
        var propertyCount = CountProperties(content);

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "JSON",
            ["PropertyCount"] = propertyCount.ToString(),
            ["CharCount"] = content.Length.ToString()
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Json,
            Text: prettyJson ?? content,
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

    private static string? FormatJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return null;
        }
    }

    private static int CountProperties(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return CountProperties(doc.RootElement);
        }
        catch
        {
            return 0;
        }
    }

    private static int CountProperties(JsonElement element)
    {
        var count = 0;
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                count++;
                count += CountProperties(prop.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
                count += CountProperties(item);
        }
        return count;
    }
}
