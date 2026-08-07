using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public interface ITextExtractorRegistry
{
    IDocumentTextExtractor? GetExtractor(string contentType);
    IDocumentTextExtractor? GetExtractor(DocumentType documentType);
    void Register(IDocumentTextExtractor extractor);
}

public sealed class TextExtractorRegistry : ITextExtractorRegistry
{
    private readonly List<IDocumentTextExtractor> _extractors;
    private readonly object _sync = new();

    public TextExtractorRegistry()
        : this(new List<IDocumentTextExtractor>
        {
            new PdfTextExtractor(),
            new DocxTextExtractor(),
            new XlsxTextExtractor(),
            new PptxTextExtractor(),
            new MarkdownTextExtractor(),
            new HtmlTextExtractor(),
            new PlainTextExtractor(),
            new CsvTextExtractor(),
            new JsonTextExtractor(),
            new XmlTextExtractor()
        })
    {
    }

    internal TextExtractorRegistry(IEnumerable<IDocumentTextExtractor> extractors)
    {
        _extractors = extractors.ToList();
    }

    public IDocumentTextExtractor? GetExtractor(string contentType)
    {
        lock (_sync)
        {
            return _extractors.FirstOrDefault(e => e.CanHandle(contentType));
        }
    }

    public IDocumentTextExtractor? GetExtractor(DocumentType documentType)
    {
        lock (_sync)
        {
            return _extractors.FirstOrDefault(e => e.SupportedType == documentType);
        }
    }

    public void Register(IDocumentTextExtractor extractor)
    {
        lock (_sync)
        {
            _extractors.RemoveAll(e => e.SupportedType == extractor.SupportedType);
            _extractors.Add(extractor);
        }
    }
}
