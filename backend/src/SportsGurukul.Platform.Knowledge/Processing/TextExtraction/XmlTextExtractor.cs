using System.Text;
using System.Xml.Linq;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class XmlTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Xml;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Xml;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var builder = new StringBuilder();

        try
        {
            var root = XDocument.Parse(DocumentContentReader.DecodeText(content, document.ContentType));
            var attributes = new List<string>();
            if (root.Root?.HasAttributes == true)
            {
                attributes.AddRange(root.Root.Attributes().Select(a => $"@{a.Name.LocalName}={a.Value}"));
            }

            foreach (var node in root.Descendants().Where(n => n.HasElements == false && !string.IsNullOrWhiteSpace(n.Value)))
            {
                var path = string.Join("/", node.AncestorsAndSelf().Reverse().Select(n => n.Name.LocalName));
                var value = node.Value.Trim();
                if (value.Length > 0)
                {
                    builder.Append(path).Append(": ").Append(value).Append('\n');
                }
            }

            var text = builder.ToString();
            var sections = new[]
            {
                new DocumentSection("Document", 1, 0, text.Length) { Content = text }
            };

            return Task.FromResult(new ExtractedDocumentText(
                text,
                sections,
                new Dictionary<string, string>
                {
                    ["root"] = root.Root?.Name.LocalName ?? string.Empty,
                    ["rootAttributes"] = string.Join(",", attributes)
                }));
        }
        catch (Exception)
        {
            var raw = DocumentContentReader.DecodeText(content, document.ContentType);
            var sections = new[]
            {
                new DocumentSection("Document", 1, 0, raw.Length) { Content = raw }
            };
            return Task.FromResult(new ExtractedDocumentText(raw, sections));
        }
    }
}
