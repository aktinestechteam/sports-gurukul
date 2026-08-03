using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class CohereEmbeddingProvider : HttpEmbeddingProviderBase, IEmbeddingProvider
{
    public string Name => "cohere";
    public int Dimensions { get; }

    private readonly EmbeddingOptions _options;

    public CohereEmbeddingProvider(IHttpClientFactory httpClientFactory, EmbeddingOptions options)
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
        var baseUrl = (_options.BaseUrl ?? "https://api.cohere.com").TrimEnd('/');
        var url = $"{baseUrl}/v1/embed";
        var body = new
        {
            model = _options.Model ?? "embed-english-v3.0",
            texts,
            input_type = "search_document",
            truncate = "END"
        };

        using var request = BuildJsonRequest(HttpMethod.Post, url, body, ("Authorization", $"Bearer {_options.ApiKey}"));
        using var response = await Client.SendAsync(request, ct);
        var rawEmbeddings = await ReadEmbeddingsAsync(response, "embeddings", ct);

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
        var baseUrl = (_options.BaseUrl ?? "https://api.cohere.com").TrimEnd('/');
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v1/models")
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
