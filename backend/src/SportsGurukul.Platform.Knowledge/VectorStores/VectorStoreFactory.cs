using System.Collections.Concurrent;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.VectorStores;

internal sealed class VectorStoreFactory : IVectorStoreFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KnowledgePlatformOptions _options;
    private readonly ConcurrentDictionary<string, IVectorStore> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Func<IVectorStore>> _customFactories = new(StringComparer.OrdinalIgnoreCase);

    public VectorStoreFactory(IHttpClientFactory httpClientFactory, KnowledgePlatformOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public IVectorStore GetStore(string? name = null)
    {
        var storeName = NormalizeName(name ?? _options.VectorStore.Provider);
        return _cache.GetOrAdd(storeName, CreateStore);
    }

    internal void RegisterCustomStore(string name, Func<IVectorStore> factory) =>
        _customFactories[NormalizeName(name)] = factory;

    private IVectorStore CreateStore(string name)
    {
        if (_customFactories.TryGetValue(name, out var factory))
        {
            return factory();
        }

        return name switch
        {
            "inmemory" => new InMemoryVectorStore(),
            "qdrant" => new QdrantVectorStore(_httpClientFactory, _options.VectorStore),
            _ => throw new NotSupportedException(
                $"Vector store '{name}' is not supported. " +
                "Supported providers: inmemory, qdrant.")
        };
    }

    private static string NormalizeName(string name) =>
        name.Trim().ToLowerInvariant()
            .Replace("ai-search", "azureaisearch")
            .Replace("_", string.Empty)
            .Replace("-", string.Empty);
}
