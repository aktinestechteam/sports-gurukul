using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface ITextExtractor
{
    Task<string> ExtractTextAsync(byte[] content, string contentType, CancellationToken cancellationToken = default);
}

public interface IMetadataExtractor
{
    Task<Dictionary<string, string>> ExtractMetadataAsync(byte[] content, string fileName, string contentType, CancellationToken cancellationToken = default);
}

public interface ILanguageDetector
{
    Task<string> DetectLanguageAsync(string text, CancellationToken cancellationToken = default);
}

public interface IOcrExtensionPoint
{
    Task<string> ExtractTextFromImageAsync(byte[] imageData, string? format, CancellationToken cancellationToken = default);
    bool IsAvailable { get; }
}

public interface IImageCaptionExtensionPoint
{
    Task<string> GenerateCaptionAsync(byte[] imageData, string? format, CancellationToken cancellationToken = default);
    bool IsAvailable { get; }
}

public interface IPiiDetectionExtensionPoint
{
    Task<PiiDetectionResult> DetectPiiAsync(string text, CancellationToken cancellationToken = default);
    bool IsAvailable { get; }
}

public record PiiDetectionResult(
    bool HasPii,
    List<PiiEntity> Entities,
    string? SanitizedText
);

public record PiiEntity(
    string Type,
    string Value,
    int StartPosition,
    int Length,
    double Confidence
);
