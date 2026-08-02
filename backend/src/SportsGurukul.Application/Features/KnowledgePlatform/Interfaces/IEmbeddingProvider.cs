using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IEmbeddingProvider
{
    EmbeddingProviderType ProviderType { get; }
    string ModelName { get; }
    int Dimensions { get; }
    bool SupportsBatchProcessing { get; }

    Task<EmbeddingVector> GenerateEmbeddingAsync(string text, string chunkId, string documentId, CancellationToken cancellationToken = default);
    Task<List<EmbeddingVector>> GenerateEmbeddingsBatchAsync(List<string> texts, string documentId, CancellationToken cancellationToken = default);
    Task<int> GetTokenCountAsync(string text, CancellationToken cancellationToken = default);
}

public interface IEmbeddingProviderFactory
{
    IEmbeddingProvider GetProvider(EmbeddingProviderType type);
    IEmbeddingProvider GetProvider(string providerName);
    bool SupportsProvider(EmbeddingProviderType type);
}
