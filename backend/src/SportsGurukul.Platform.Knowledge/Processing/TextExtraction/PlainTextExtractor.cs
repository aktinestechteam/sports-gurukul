using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class PlainTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Text;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Text;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        var text = DocumentContentReader.DecodeText(content, document.ContentType);
        var sections = new[]
        {
            new DocumentSection("Document", 1, 0, text.Length) { Content = text }
        };
        return Task.FromResult(new ExtractedDocumentText(text, sections));
    }
}
