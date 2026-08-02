using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IDocumentParser
{
    DocumentFormat SupportedFormat { get; }
    Task<ExtractedDocument> ParseAsync(RawDocument document, CancellationToken cancellationToken = default);
}

public interface IDocumentParserFactory
{
    IDocumentParser GetParser(DocumentFormat format);
    bool SupportsFormat(DocumentFormat format);
}
