using System.Text;
using System.Xml;
using System.Xml.Linq;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class XmlParser : IDocumentParser
{
    public DocumentFormat SupportedFormat => DocumentFormat.Xml;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var content = System.Text.Encoding.UTF8.GetString(document.Content);

        var text = ExtractTextFromXml(content);
        var rootElement = ExtractRootElement(content);

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "XML",
            ["RootElement"] = rootElement ?? "unknown",
            ["CharCount"] = content.Length.ToString()
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Xml,
            Text: text,
            Title: rootElement ?? Path.GetFileNameWithoutExtension(document.FileName),
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

    private static string ExtractTextFromXml(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var sb = new StringBuilder();
            ExtractTextNodes(doc.Root!, sb);
            return sb.ToString().Trim();
        }
        catch
        {
            return xml;
        }
    }

    private static void ExtractTextNodes(XElement element, StringBuilder sb)
    {
        if (!element.HasElements)
        {
            var text = element.Value.Trim();
            if (!string.IsNullOrEmpty(text))
                sb.AppendLine($"{element.Name.LocalName}: {text}");
        }
        else
        {
            foreach (var child in element.Elements())
                ExtractTextNodes(child, sb);
        }
    }

    private static string? ExtractRootElement(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            return doc.Root?.Name.LocalName;
        }
        catch
        {
            return null;
        }
    }
}
