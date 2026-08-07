using System.Collections.Concurrent;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class EmbeddingProviderFactory : IEmbeddingProviderFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KnowledgePlatformOptions _options;
    private readonly ConcurrentDictionary<string, IEmbeddingProvider> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Func<IEmbeddingProvider>> _customFactories = new(StringComparer.OrdinalIgnoreCase);

    public EmbeddingProviderFactory(IHttpClientFactory httpClientFactory, KnowledgePlatformOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public IEmbeddingProvider GetProvider(string? name = null)
    {
        var providerName = NormalizeName(name ?? _options.Embedding.Provider);
        return _cache.GetOrAdd(providerName, CreateProvider);
    }

    internal void RegisterCustomProvider(string name, Func<IEmbeddingProvider> factory) =>
        _customFactories[NormalizeName(name)] = factory;

    private IEmbeddingProvider CreateProvider(string name)
    {
        if (_customFactories.TryGetValue(name, out var factory))
        {
            return factory();
        }

        var options = _options.Embedding;
        return name switch
        {
            "deterministic" => new DeterministicEmbeddingProvider(options.Dimensions),
            "openai" => new OpenAiEmbeddingProvider(_httpClientFactory, options),
            "azureopenai" => new AzureOpenAiEmbeddingProvider(_httpClientFactory, options),
            "gemini" => new GeminiEmbeddingProvider(_httpClientFactory, options),
            "cohere" => new CohereEmbeddingProvider(_httpClientFactory, options),
            "ollama" => new OllamaEmbeddingProvider(_httpClientFactory, options),
            _ => throw new NotSupportedException(
                $"Embedding provider '{name}' is not supported. " +
                "Supported providers: deterministic, openai, azureopenai, gemini, cohere, ollama.")
        };
    }

    private static string NormalizeName(string name) =>
        name.Trim().ToLowerInvariant()
            .Replace("azure-openai", "azureopenai")
            .Replace("azure_openai", "azureopenai")
            .Replace("-", string.Empty);
}
