using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class PdfParser : IDocumentParser
{
    public DocumentFormat SupportedFormat => DocumentFormat.Pdf;

    public async Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var text = await ExtractTextFromPdfAsync(document.Content, cancellationToken);
        var metadata = await ExtractPdfMetadataAsync(document.Content, cancellationToken);

        return new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Pdf,
            Text: text,
            Title: metadata.GetValueOrDefault("Title"),
            PageCount: metadata.TryGetValue("PageCount", out var pc) && int.TryParse(pc, out var p) ? p : null,
            DetectedLanguage: null,
            Author: metadata.GetValueOrDefault("Author"),
            CreatedDate: null,
            ModifiedDate: null,
            Metadata: metadata,
            Images: null,
            Status: ProcessingStatus.Extracted
        );
    }

    private Task<string> ExtractTextFromPdfAsync(byte[] content, CancellationToken ct)
    {
        return Task.FromResult("[PDF content requires PdfPig or iTextSharp integration]");
    }

    private Task<Dictionary<string, string>> ExtractPdfMetadataAsync(byte[] content, CancellationToken ct)
    {
        return Task.FromResult(new Dictionary<string, string>
        {
            ["PageCount"] = "0",
            ["Format"] = "PDF"
        });
    }
}
