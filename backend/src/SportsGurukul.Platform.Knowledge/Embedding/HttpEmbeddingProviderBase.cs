using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal abstract class HttpEmbeddingProviderBase
{
    protected HttpEmbeddingProviderBase(IHttpClientFactory httpClientFactory, EmbeddingOptions options)
    {
        Client = httpClientFactory.CreateClient("KnowledgePlatform.Embedding");
        Client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
    }

    protected HttpClient Client { get; }

    protected static async Task<IReadOnlyList<float[]>> ReadEmbeddingsAsync(
        HttpResponseMessage response,
        string rootProperty,
        CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Embedding request failed with status {(int)response.StatusCode}: {errorBody}");
        }

        using var doc = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
        var root = doc.RootElement;

        if (!root.TryGetProperty(rootProperty, out var embeddingsElement))
        {
            throw new InvalidOperationException($"Embedding response missing '{rootProperty}'.");
        }

        var result = new List<float[]>(embeddingsElement.GetArrayLength());
        foreach (var item in embeddingsElement.EnumerateArray())
        {
            var values = new float[item.GetArrayLength()];
            var i = 0;
            foreach (var number in item.EnumerateArray())
            {
                values[i++] = number.GetSingle();
            }

            result.Add(values);
        }

        return result;
    }

    protected static HttpRequestMessage BuildJsonRequest(HttpMethod method, string url, object body, params (string Name, string Value)[] headers)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        foreach (var (name, value) in headers)
        {
            request.Headers.TryAddWithoutValidation(name, value);
        }

        return request;
    }
}
