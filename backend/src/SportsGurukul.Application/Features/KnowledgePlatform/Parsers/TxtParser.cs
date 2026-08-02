using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class TxtParser : IDocumentParser
{
    public DocumentFormat SupportedFormat => DocumentFormat.PlainText;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var text = System.Text.Encoding.UTF8.GetString(document.Content);

        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "PlainText",
            ["LineCount"] = text.Split('\n').Length.ToString(),
            ["CharCount"] = text.Length.ToString()
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.PlainText,
            Text: text,
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
}
