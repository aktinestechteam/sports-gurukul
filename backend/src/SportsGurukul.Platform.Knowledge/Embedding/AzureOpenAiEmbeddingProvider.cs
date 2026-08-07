using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class AzureOpenAiEmbeddingProvider : HttpEmbeddingProviderBase, IEmbeddingProvider
{
    public string Name => "azureopenai";
    public int Dimensions { get; }

    private readonly EmbeddingOptions _options;
    private const string ApiVersion = "2024-06-01";

    public AzureOpenAiEmbeddingProvider(IHttpClientFactory httpClientFactory, EmbeddingOptions options)
        : base(httpClientFactory, options)
    {
        _options = options;
        Dimensions = options.Dimensions;
    }

    public async Task<EmbeddingVector> EmbedAsync(string text, CancellationToken ct = default)
    {
        var results = await EmbedBatchAsync(new[] { text }, ct);
        return results[0];
    }

    public async Task<IReadOnlyList<EmbeddingVector>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        var baseUrl = (_options.BaseUrl ?? "https://<resource>.openai.azure.com").TrimEnd('/');
        var deployment = _options.DeploymentName ?? _options.Model ?? "text-embedding";
        var url = $"{baseUrl}/openai/deployments/{deployment}/embeddings?api-version={ApiVersion}";
        var body = new { input = texts };

        using var request = BuildJsonRequest(HttpMethod.Post, url, body, ("api-key", _options.ApiKey ?? string.Empty));
        using var response = await Client.SendAsync(request, ct);
        var rawEmbeddings = await ReadEmbeddingsAsync(response, "data", ct);

        if (rawEmbeddings.Count != texts.Count)
        {
            throw new InvalidOperationException(
                $"Embedding response count ({rawEmbeddings.Count}) does not match request count ({texts.Count}).");
        }

        return rawEmbeddings
            .Select(e => new EmbeddingVector(e, e.Length))
            .ToList();
    }

    public Task<bool> IsHealthyAsync(CancellationToken ct = default)
    {
        var baseUrl = (_options.BaseUrl ?? "https://<resource>.openai.azure.com").TrimEnd('/');
        var deployment = _options.DeploymentName ?? _options.Model ?? "text-embedding";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/openai/deployments/{deployment}?api-version={ApiVersion}")
        {
            Headers = { { "api-key", _options.ApiKey ?? string.Empty } }
        };

        return InvokeHealthCheckAsync(request, ct);
    }

    private async Task<bool> InvokeHealthCheckAsync(HttpRequestMessage request, CancellationToken ct)
    {
        try
        {
            using var response = await Client.SendAsync(request, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
