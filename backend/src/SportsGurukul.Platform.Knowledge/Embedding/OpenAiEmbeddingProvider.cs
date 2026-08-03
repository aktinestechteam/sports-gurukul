using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class OpenAiEmbeddingProvider : HttpEmbeddingProviderBase, IEmbeddingProvider
{
    public string Name => "openai";
    public int Dimensions { get; }

    private readonly EmbeddingOptions _options;

    public OpenAiEmbeddingProvider(IHttpClientFactory httpClientFactory, EmbeddingOptions options)
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
        var baseUrl = (_options.BaseUrl ?? "https://api.openai.com/v1").TrimEnd('/');
        var url = $"{baseUrl}/embeddings";
        var body = new
        {
            model = _options.Model ?? "text-embedding-3-small",
            input = texts
        };

        using var request = BuildJsonRequest(HttpMethod.Post, url, body, ("Authorization", $"Bearer {_options.ApiKey}"));
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
        var baseUrl = (_options.BaseUrl ?? "https://api.openai.com/v1").TrimEnd('/');
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/models")
        {
            Headers = { { "Authorization", $"Bearer {_options.ApiKey}" } }
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
