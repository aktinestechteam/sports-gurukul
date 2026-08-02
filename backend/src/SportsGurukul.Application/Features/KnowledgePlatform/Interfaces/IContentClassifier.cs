namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IContentClassifier
{
    Task<string> ClassifyAsync(string text, CancellationToken cancellationToken = default);
    Task<Dictionary<string, double>> ClassifyWithScoresAsync(string text, CancellationToken cancellationToken = default);
}

public interface IDocumentFingerprinter
{
    Task<string> ComputeChecksumAsync(byte[] content, CancellationToken cancellationToken = default);
    Task<string> ComputeContentHashAsync(string text, CancellationToken cancellationToken = default);
}

public interface IDeduplicationService
{
    Task<bool> IsDuplicateAsync(string documentId, string checksum, CancellationToken cancellationToken = default);
    Task<bool> IsContentDuplicateAsync(string contentHash, CancellationToken cancellationToken = default);
    Task MarkAsIndexedAsync(string documentId, string checksum, string contentHash, CancellationToken cancellationToken = default);
}
