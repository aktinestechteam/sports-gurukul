using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed partial class DocumentProcessor : IDocumentProcessor
{
    private readonly ITextExtractorRegistry _extractorRegistry;
    private readonly ILanguageDetector _languageDetector;
    private readonly IPiiDetector _piiDetector;
    private readonly IContentClassifier _contentClassifier;
    private readonly IDocumentFingerprinter _fingerprinter;

    public DocumentProcessor(
        ITextExtractorRegistry extractorRegistry,
        ILanguageDetector languageDetector,
        IPiiDetector piiDetector,
        IContentClassifier contentClassifier,
        IDocumentFingerprinter fingerprinter)
    {
        _extractorRegistry = extractorRegistry;
        _languageDetector = languageDetector;
        _piiDetector = piiDetector;
        _contentClassifier = contentClassifier;
        _fingerprinter = fingerprinter;
    }

    public async Task<ProcessedDocument> ProcessAsync(
        KnowledgeDocument document,
        byte[]? contentOverride = null,
        CancellationToken ct = default)
    {
        try
        {
            var extractor = _extractorRegistry.GetExtractor(document.ContentType)
                            ?? _extractorRegistry.GetExtractor(document.DocumentType);

            if (extractor == null)
            {
                return Failed(document, DocumentIngestionState.Failed,
                    $"No text extractor registered for content type '{document.ContentType}'.");
            }

            var content = contentOverride
                          ?? await DocumentContentReader.ReadBytesAsync(document, ct);

            var extracted = await extractor.ExtractAsync(document, content, ct);
            var normalized = Normalize(extracted.Text);
            var language = document.Language
                           ?? _languageDetector.Detect(normalized);

            var piiFindings = await _piiDetector.DetectAsync(normalized, ct);
            var safeText = _piiDetector.Redact(normalized, piiFindings);
            var classification = await _contentClassifier.ClassifyAsync(safeText, ct);
            var fingerprint = _fingerprinter.Compute(normalized);

            var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (extracted.Metadata is not null)
            {
                foreach (var (key, value) in extracted.Metadata)
                {
                    metadata[key] = value;
                }
            }

            metadata["documentType"] = document.DocumentType.ToString();
            metadata["language"] = language;
            metadata["classification"] = classification.Category;
            metadata["fileName"] = document.FileName ?? document.Title;
            metadata["sizeBytes"] = document.SizeBytes.ToString();

            return new ProcessedDocument(
                document,
                normalized,
                extracted.Sections,
                language,
                classification,
                fingerprint,
                piiFindings,
                safeText,
                metadata,
                DocumentIngestionState.Extracted);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Failed(document, DocumentIngestionState.Failed, ex.Message);
        }
    }

    internal static string Normalize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var normalized = text.Replace('\u00A0', ' ').Replace("\r\n", "\n").Replace('\r', '\n');
        normalized = MultiNewlineRegex().Replace(normalized, "\n");
        normalized = WhitespaceRegex().Replace(normalized, " ");
        return normalized.Trim();
    }

    private static ProcessedDocument Failed(KnowledgeDocument document, DocumentIngestionState state, string error) =>
        new(
            document,
            string.Empty,
            Array.Empty<DocumentSection>(),
            string.Empty,
            new ContentClassification("Unknown", 0),
            new DocumentFingerprint("none", string.Empty),
            Array.Empty<PiiFinding>(),
            string.Empty,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            state,
            error);

    [GeneratedRegex(@"[ \t]{2,}")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex MultiNewlineRegex();
}
