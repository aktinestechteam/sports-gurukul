using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Embedding;

public class EmbeddingProviderFactory : IEmbeddingProviderFactory
{
    private readonly Dictionary<EmbeddingProviderType, IEmbeddingProvider> _providers;
    private readonly Dictionary<string, IEmbeddingProvider> _namedProviders;

    public EmbeddingProviderFactory(IEnumerable<IEmbeddingProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.ProviderType);
        _namedProviders = providers.ToDictionary(p => p.ProviderType.ToString(), StringComparer.OrdinalIgnoreCase);
    }

    public IEmbeddingProvider GetProvider(EmbeddingProviderType type) =>
        _providers.TryGetValue(type, out var provider) ? provider
        : throw new NotSupportedException($"No embedding provider registered for type: {type}");

    public IEmbeddingProvider GetProvider(string providerName) =>
        _namedProviders.TryGetValue(providerName, out var provider) ? provider
        : throw new NotSupportedException($"No embedding provider registered with name: {providerName}");

    public bool SupportsProvider(EmbeddingProviderType type) =>
        _providers.ContainsKey(type);
}
