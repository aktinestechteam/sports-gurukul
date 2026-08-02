using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class DocumentProcessingService : IDocumentProcessor
{
    private readonly IDocumentParserFactory _parserFactory;
    private readonly ITextExtractor? _textExtractor;
    private readonly IMetadataExtractor? _metadataExtractor;
    private readonly ILanguageDetector? _languageDetector;
    private readonly IOcrExtensionPoint? _ocrExtension;
    private readonly IImageCaptionExtensionPoint? _imageCaptionExtension;
    private readonly IPiiDetectionExtensionPoint? _piiExtension;
    private readonly IContentClassifier? _contentClassifier;
    private readonly IDocumentFingerprinter? _fingerprinter;
    private readonly ILogger<DocumentProcessingService> _logger;

    public DocumentProcessingService(
        IDocumentParserFactory parserFactory,
        ILogger<DocumentProcessingService> logger,
        ITextExtractor? textExtractor = null,
        IMetadataExtractor? metadataExtractor = null,
        ILanguageDetector? languageDetector = null,
        IOcrExtensionPoint? ocrExtension = null,
        IImageCaptionExtensionPoint? imageCaptionExtension = null,
        IPiiDetectionExtensionPoint? piiExtension = null,
        IContentClassifier? contentClassifier = null,
        IDocumentFingerprinter? fingerprinter = null)
    {
        _parserFactory = parserFactory;
        _logger = logger;
        _textExtractor = textExtractor;
        _metadataExtractor = metadataExtractor;
        _languageDetector = languageDetector;
        _ocrExtension = ocrExtension;
        _imageCaptionExtension = imageCaptionExtension;
        _piiExtension = piiExtension;
        _contentClassifier = contentClassifier;
        _fingerprinter = fingerprinter;
    }

    public async Task<ExtractedDocument> ProcessAsync(RawDocument document, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing document {DocumentId} ({FileName})", document.Id, document.FileName);

        var parser = _parserFactory.GetParser(document.Format);
        var extracted = await parser.ParseAsync(document, cancellationToken);

        if (_languageDetector != null && extracted.DetectedLanguage == null)
        {
            var lang = await _languageDetector.DetectLanguageAsync(extracted.Text, cancellationToken);
            extracted = extracted with { DetectedLanguage = lang };
        }

        if (_contentClassifier != null && extracted.Metadata != null)
        {
            var classification = await _contentClassifier.ClassifyAsync(extracted.Text, cancellationToken);
            extracted.Metadata["classification"] = classification;
        }

        _logger.LogInformation("Document {DocumentId} processed: {TextLength} chars, format: {Format}",
            document.Id, extracted.Text.Length, extracted.Format);

        return extracted;
    }

    public async Task<ExtractedDocument> ProcessWithExtensionsAsync(
        RawDocument document,
        bool enableOcr,
        bool enableImageCaptioning,
        bool enablePiiDetection,
        CancellationToken cancellationToken = default)
    {
        var extracted = await ProcessAsync(document, cancellationToken);

        if (enableOcr && _ocrExtension?.IsAvailable == true && extracted.Images != null)
        {
            foreach (var image in extracted.Images)
            {
                var ocrText = await _ocrExtension.ExtractTextFromImageAsync(image.Data!, image.Format, cancellationToken);
                extracted = extracted with { Text = extracted.Text + "\n" + ocrText };
            }
        }

        if (enableImageCaptioning && _imageCaptionExtension?.IsAvailable == true && extracted.Images != null)
        {
            foreach (var image in extracted.Images)
            {
                var caption = await _imageCaptionExtension.GenerateCaptionAsync(image.Data!, image.Format, cancellationToken);
                extracted = extracted with { Images = new List<ExtractedImage>(extracted.Images ?? []) };
            }
        }

        if (enablePiiDetection && _piiExtension?.IsAvailable == true)
        {
            var piiResult = await _piiExtension.DetectPiiAsync(extracted.Text, cancellationToken);
            if (piiResult.HasPii && piiResult.SanitizedText != null)
                extracted = extracted with { Text = piiResult.SanitizedText };
        }

        return extracted;
    }
}
