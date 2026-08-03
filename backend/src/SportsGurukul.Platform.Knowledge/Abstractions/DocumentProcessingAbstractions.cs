using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public interface IDocumentTextExtractor
{
    DocumentType SupportedType { get; }
    bool CanHandle(string contentType);
    Task<ExtractedDocumentText> ExtractAsync(KnowledgeDocument document, byte[] content, CancellationToken ct = default);
}

public interface IDocumentMetadataExtractor
{
    Task<IReadOnlyDictionary<string, string>> ExtractAsync(KnowledgeDocument document, CancellationToken ct = default);
}

public interface ILanguageDetector
{
    string Detect(string text);
}

public interface IOcrEngine
{
    string Name { get; }
    bool CanHandle(string contentType);
    Task<string> ExtractTextAsync(KnowledgeDocument document, CancellationToken ct = default);
}

public interface IImageCaptioner
{
    string Name { get; }
    bool CanHandle(string contentType);
    Task<string> CaptionAsync(KnowledgeDocument document, CancellationToken ct = default);
}

public interface IPiiDetector
{
    string Name { get; }
    Task<IReadOnlyList<PiiFinding>> DetectAsync(string text, CancellationToken ct = default);
    string Redact(string text, IReadOnlyList<PiiFinding> findings);
}

public interface IContentClassifier
{
    string Name { get; }
    Task<ContentClassification> ClassifyAsync(string text, CancellationToken ct = default);
}

public interface IDocumentFingerprinter
{
    string Algorithm { get; }
    DocumentFingerprint Compute(string normalizedText);
}

public interface IDeduplicator
{
    string Name { get; }
    Task<string?> FindDuplicateAsync(
        DocumentFingerprint fingerprint,
        string indexName,
        string tenantId,
        IReadOnlyList<string> existingFingerprints,
        CancellationToken ct = default);
}

public interface IDocumentProcessor
{
    Task<ProcessedDocument> ProcessAsync(
        KnowledgeDocument document,
        byte[]? contentOverride = null,
        CancellationToken ct = default);
}
