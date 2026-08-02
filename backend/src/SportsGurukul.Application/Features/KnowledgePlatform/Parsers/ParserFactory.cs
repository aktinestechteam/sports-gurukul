using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Parsers;

public class DocumentParserFactory : IDocumentParserFactory
{
    private readonly Dictionary<DocumentFormat, IDocumentParser> _parsers;

    public DocumentParserFactory(IEnumerable<IDocumentParser> parsers)
    {
        _parsers = parsers.ToDictionary(p => p.SupportedFormat);
    }

    public IDocumentParser GetParser(DocumentFormat format) =>
        _parsers.TryGetValue(format, out var parser) ? parser
        : throw new NotSupportedException($"No parser registered for format: {format}");

    public bool SupportsFormat(DocumentFormat format) =>
        _parsers.ContainsKey(format);
}
