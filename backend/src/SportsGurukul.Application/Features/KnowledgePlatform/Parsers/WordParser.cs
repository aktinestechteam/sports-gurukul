using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class WordParser : IDocumentParser
{
    public DocumentFormat SupportedFormat => DocumentFormat.Word;

    public Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        var metadata = new Dictionary<string, string>
        {
            ["Format"] = "Word",
            ["Extension"] = Path.GetExtension(document.FileName)
        };

        return Task.FromResult(new ExtractedDocument(
            Id: document.Id,
            FileName: document.FileName,
            Format: DocumentFormat.Word,
            Text: "[Word content requires DocumentFormat.OpenXml integration]",
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
