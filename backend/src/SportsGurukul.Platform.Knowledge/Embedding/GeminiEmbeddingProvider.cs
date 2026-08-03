using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class GeminiEmbeddingProvider : HttpEmbeddingProviderBase, IEmbeddingProvider
{
    public string Name => "gemini";
    public int Dimensions { get; }

    private readonly EmbeddingOptions _options;

    public GeminiEmbeddingProvider(IHttpClientFactory httpClientFactory, EmbeddingOptions options)
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
        var baseUrl = (_options.BaseUrl ?? "https://generativelanguage.googleapis.com").TrimEnd('/');
        var model = _options.Model ?? "text-embedding-004";
        var apiKey = _options.ApiKey ?? string.Empty;
        var results = new List<EmbeddingVector>(texts.Count);

        foreach (var text in texts)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"{baseUrl}/v1beta/models/{model}:embedContent?key={Uri.EscapeDataString(apiKey)}";
            var body = new
            {
                content = new { parts = new object[] { new { text } } },
                taskType = "RETRIEVAL_DOCUMENT"
            };

            using var request = BuildJsonRequest(HttpMethod.Post, url, body);
            using var response = await Client.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException(
                    $"Embedding request failed with status {(int)response.StatusCode}: {errorBody}");
            }

            using var doc = await System.Text.Json.JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            if (!doc.RootElement.TryGetProperty("embedding", out var embedding)
                || !embedding.TryGetProperty("values", out var values))
            {
                throw new InvalidOperationException("Embedding response missing 'embedding.values'.");
            }

            var vector = new float[values.GetArrayLength()];
            var i = 0;
            foreach (var number in values.EnumerateArray())
            {
                vector[i++] = number.GetSingle();
            }

            results.Add(new EmbeddingVector(vector, vector.Length));
        }

        return results;
    }

    public Task<bool> IsHealthyAsync(CancellationToken ct = default)
    {
        var baseUrl = (_options.BaseUrl ?? "https://generativelanguage.googleapis.com").TrimEnd('/');
        var model = _options.Model ?? "text-embedding-004";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v1beta/models/{model}?key={Uri.EscapeDataString(_options.ApiKey ?? string.Empty)}");
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
