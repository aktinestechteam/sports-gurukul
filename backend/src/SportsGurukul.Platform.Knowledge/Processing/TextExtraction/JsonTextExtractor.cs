using System.Text;
using System.Text.Json;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class JsonTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Json;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Json;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var builder = new StringBuilder();

        using (var doc = JsonDocument.Parse(content))
        {
            Flatten(doc.RootElement, "$", builder);
        }

        var text = builder.ToString();
        var sections = new[]
        {
            new DocumentSection("Document", 1, 0, text.Length) { Content = text }
        };

        return Task.FromResult(new ExtractedDocumentText(text, sections));
    }

    private static void Flatten(JsonElement element, string path, StringBuilder builder)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var childPath = path == "$" ? $"$.{property.Name}" : $"{path}.{property.Name}";
                    Flatten(property.Value, childPath, builder);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    Flatten(item, $"{path}[{index}]", builder);
                    index++;
                }
                break;

            case JsonValueKind.String:
                builder.AppendLine($"{path}: {element.GetString()}");
                break;

            case JsonValueKind.Null:
                builder.AppendLine($"{path}: null");
                break;

            default:
                builder.AppendLine($"{path}: {element}");
                break;
        }
    }
}
