using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.VectorStores;

internal sealed class QdrantVectorStore : IVectorStore
{
    public string Name => "qdrant";
    public VectorStoreCapabilities Capabilities => new(true, false, true);

    private readonly HttpClient _client;
    private readonly VectorStoreOptions _options;
    private const int ScrollLimit = 5000;

    public QdrantVectorStore(IHttpClientFactory httpClientFactory, VectorStoreOptions options)
    {
        _client = httpClientFactory.CreateClient("KnowledgePlatform.Qdrant");
        _client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
        _options = options;
    }

    private string BaseUrl => (_options.BaseUrl ?? "http://localhost:6333").TrimEnd('/');

    public async Task UpsertAsync(ChunkEmbedding embedding, CancellationToken ct = default) =>
        await UpsertBatchAsync(new[] { embedding }, ct);

    public async Task UpsertBatchAsync(IReadOnlyList<ChunkEmbedding> embeddings, CancellationToken ct = default)
    {
        if (embeddings.Count == 0)
        {
            return;
        }

        var collection = CollectionName(embeddings[0].IndexName);
        await EnsureCollectionAsync(collection, embeddings[0].Vector.Dimensions, ct);

        var points = embeddings.Select(e => new
        {
            id = e.ChunkId.ToString(),
            vector = e.Vector.Values,
            payload = BuildPayload(e)
        }).ToList();

        var url = $"{BaseUrl}/collections/{collection}/points?wait=true";
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(new { points }), Encoding.UTF8, "application/json")
        };

        using var response = await _client.SendAsync(request, ct);
        await EnsureSuccessAsync(response, "upsert points", ct);
    }

    public async Task<bool> DeleteAsync(Guid chunkId, CancellationToken ct = default)
    {
        var id = chunkId.ToString();
        foreach (var collection in await ListCollectionsAsync(ct))
        {
            var url = $"{BaseUrl}/collections/{collection}/points/delete";
            var body = new { points = new[] { id } };
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(request, ct);
            await EnsureSuccessAsync(response, "delete point", ct);
        }

        return true;
    }

    public async Task DeleteBatchAsync(IReadOnlyList<Guid> chunkIds, CancellationToken ct = default)
    {
        if (chunkIds.Count == 0)
        {
            return;
        }

        foreach (var collection in await ListCollectionsAsync(ct))
        {
            var url = $"{BaseUrl}/collections/{collection}/points/delete";
            var body = new { points = chunkIds.Select(id => id.ToString()).ToArray() };
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(request, ct);
            await EnsureSuccessAsync(response, "delete points", ct);
        }
    }

    public async Task<int> DeleteByFilterAsync(VectorFilter filter, CancellationToken ct = default)
    {
        var collection = CollectionName(filter.IndexName);
        if (!await CollectionExistsAsync(collection, ct))
        {
            return 0;
        }

        var url = $"{BaseUrl}/collections/{collection}/points/delete";
        var body = new { filter = BuildFilter(filter) };
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        using var response = await _client.SendAsync(request, ct);
        await EnsureSuccessAsync(response, "delete points by filter", ct);
        return -1;
    }

    public async Task<IReadOnlyList<RetrievedChunk>> SearchAsync(VectorSearchQuery query, CancellationToken ct = default)
    {
        var collection = CollectionName(query.Filter.IndexName);
        var url = $"{BaseUrl}/collections/{collection}/points/search";
        var body = new
        {
            vector = query.Vector.Values,
            limit = Math.Max(1, query.TopK),
            with_payload = true,
            score_threshold = query.MinScore,
            filter = BuildFilter(query.Filter)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        using var response = await _client.SendAsync(request, ct);
        await EnsureSuccessAsync(response, "search points", ct);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
        var results = new List<RetrievedChunk>();
        var rank = 0;
        foreach (var item in doc.RootElement.GetProperty("result").EnumerateArray())
        {
            var chunk = TryParseChunk(item, query.Filter.IndexName);
            if (chunk is null)
            {
                continue;
            }

            var score = item.GetProperty("score").GetSingle();
            results.Add(new RetrievedChunk(chunk, score, rank, RetrievalStrategy.Semantic));
            rank++;
        }

        return results;
    }

    public async Task<IReadOnlyList<RetrievedChunk>> SearchByTextAsync(KeywordSearchQuery query, CancellationToken ct = default)
    {
        var collection = CollectionName(query.Filter.IndexName);
        if (!await CollectionExistsAsync(collection, ct))
        {
            return Array.Empty<RetrievedChunk>();
        }

        var queryTokens = VectorMath.Tokenize(query.QueryText);
        if (queryTokens.Count == 0)
        {
            return Array.Empty<RetrievedChunk>();
        }

        var points = await ScrollPointsAsync(collection, query.Filter, ct);
        var documents = points.Select(p => p.Chunk).ToList();
        var documentFrequency = new Dictionary<string, int>(StringComparer.Ordinal);
        var totalLength = 0.0;
        foreach (var document in documents)
        {
            var tokens = VectorMath.Tokenize(document.Text).Distinct();
            totalLength += Math.Max(1, VectorMath.Tokenize(document.Text).Count);
            foreach (var token in tokens)
            {
                documentFrequency[token] = documentFrequency.TryGetValue(token, out var count) ? count + 1 : 1;
            }
        }

        var averageLength = documents.Count == 0 ? 1.0 : totalLength / documents.Count;
        var excluded = query.ExcludeChunkIds?.ToHashSet() ?? new HashSet<Guid>();
        var scored = new List<(RetrievedChunk Chunk, float Score)>();

        foreach (var (chunk, score) in points)
        {
            if (excluded.Contains(chunk.Id))
            {
                continue;
            }

            var bm25 = Bm25Score(chunk.Text, queryTokens, documentFrequency, documents.Count, averageLength);
            if (bm25 >= query.MinScore)
            {
                scored.Add((new RetrievedChunk(chunk, bm25, 0, RetrievalStrategy.Keyword), bm25));
            }
        }

        return scored
            .OrderByDescending(s => s.Score)
            .Take(Math.Max(0, query.TopK))
            .Select((s, i) => s.Chunk with { Rank = i })
            .ToList();
    }

    public async Task<long> CountAsync(string? indexName = null, CancellationToken ct = default)
    {
        if (indexName is not null)
        {
            return await CountCollectionAsync(CollectionName(indexName), ct);
        }

        long total = 0;
        foreach (var collection in await ListCollectionsAsync(ct))
        {
            total += await CountCollectionAsync(collection, ct);
        }

        return total;
    }

    public async Task ResetAsync(string? indexName = null, CancellationToken ct = default)
    {
        var collections = indexName is null
            ? await ListCollectionsAsync(ct)
            : new[] { CollectionName(indexName) };

        foreach (var collection in collections)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/collections/{collection}");
            using var response = await _client.SendAsync(request, ct);
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken ct = default)
    {
        try
        {
            using var response = await _client.GetAsync($"{BaseUrl}/collections", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private string CollectionName(string indexName)
    {
        var prefix = _options.CollectionPrefix ?? "kg";
        var safeIndex = indexName.Trim().ToLowerInvariant().Replace(' ', '-');
        return $"{prefix}_{safeIndex}";
    }

    private async Task EnsureCollectionAsync(string collection, int dimensions, CancellationToken ct)
    {
        if (await CollectionExistsAsync(collection, ct))
        {
            return;
        }

        var body = new
        {
            vectors = new
            {
                size = Math.Max(1, dimensions),
                distance = "Cosine"
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/collections/{collection}")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        using var response = await _client.SendAsync(request, ct);
        await EnsureSuccessAsync(response, "create collection", ct);
    }

    private async Task<bool> CollectionExistsAsync(string collection, CancellationToken ct)
    {
        try
        {
            using var response = await _client.GetAsync($"{BaseUrl}/collections/{collection}", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken ct)
    {
        try
        {
            using var response = await _client.GetAsync($"{BaseUrl}/collections", ct);
            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<string>();
            }

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            if (!doc.RootElement.TryGetProperty("result", out var result))
            {
                return Array.Empty<string>();
            }

            var prefix = _options.CollectionPrefix is null ? null : _options.CollectionPrefix + "_";
            var collections = new List<string>();
            foreach (var item in result.EnumerateArray())
            {
                var name = item.GetProperty("name").GetString()!;
                if (prefix is null || name.StartsWith(prefix, StringComparison.Ordinal))
                {
                    collections.Add(name);
                }
            }

            return collections;
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private async Task<long> CountCollectionAsync(string collection, CancellationToken ct)
    {
        if (!await CollectionExistsAsync(collection, ct))
        {
            return 0;
        }

        var url = $"{BaseUrl}/collections/{collection}/points/count";
        var body = new { exact = true };
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        try
        {
            using var response = await _client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            return doc.RootElement.GetProperty("result").GetProperty("count").GetInt64();
        }
        catch
        {
            return 0;
        }
    }

    private async Task<IReadOnlyList<(DocumentChunk Chunk, float Score)>> ScrollPointsAsync(
        string collection,
        VectorFilter filter,
        CancellationToken ct)
    {
        var url = $"{BaseUrl}/collections/{collection}/points/scroll";
        var body = new
        {
            limit = ScrollLimit,
            with_payload = true,
            with_vector = false,
            filter = BuildFilter(filter)
        };

        var points = new List<(DocumentChunk Chunk, float Score)>();
        using (var request = new HttpRequestMessage(HttpMethod.Post, url)
               {
                   Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
               })
        {
            using var response = await _client.SendAsync(request, ct);
            await EnsureSuccessAsync(response, "scroll points", ct);

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            foreach (var item in doc.RootElement.GetProperty("result").GetProperty("points").EnumerateArray())
            {
                var chunk = TryParseChunk(item, filter.IndexName);
                if (chunk is not null)
                {
                    points.Add((chunk, 0f));
                }
            }
        }

        return points;
    }

    private static object BuildPayload(ChunkEmbedding embedding)
    {
        var chunk = embedding.Chunk;
        var metadata = new Dictionary<string, string>(chunk.Metadata, StringComparer.Ordinal);
        metadata["document_id"] = chunk.DocumentId.ToString();
        metadata["document_title"] = metadata.TryGetValue("document_title", out var title)
            ? title
            : string.Empty;

        return new
        {
            chunk_id = chunk.Id.ToString(),
            document_id = chunk.DocumentId.ToString(),
            index_name = chunk.IndexName,
            tenant_id = embedding.TenantId,
            owner_user_id = embedding.OwnerUserId,
            order = chunk.Order,
            page_number = chunk.PageNumber,
            heading = chunk.Heading,
            section = chunk.Section,
            parent_chunk_id = chunk.ParentChunkId,
            text = chunk.Text,
            token_count = chunk.TokenCount,
            metadata
        };
    }

    private static DocumentChunk? TryParseChunk(JsonElement item, string indexName)
    {
        if (!item.TryGetProperty("payload", out var payload))
        {
            return null;
        }

        var metadata = new Dictionary<string, string>(StringComparer.Ordinal);
        if (payload.TryGetProperty("metadata", out var metadataElement))
        {
            foreach (var property in metadataElement.EnumerateObject())
            {
                metadata[property.Name] = property.Value.GetString() ?? string.Empty;
            }
        }

        if (metadata.TryGetValue("document_title", out var title))
        {
            metadata["document_title"] = title;
        }

        if (metadata.TryGetValue("document_id", out var documentId))
        {
            metadata["document_id"] = documentId;
        }

        if (metadata.TryGetValue("source_link", out var sourceLink))
        {
            metadata["source_link"] = sourceLink;
        }

        var chunkId = payload.TryGetProperty("chunk_id", out var chunkIdElement)
            && Guid.TryParse(chunkIdElement.GetString(), out var parsedChunkId)
            ? parsedChunkId
            : Guid.Empty;

        var parsedDocId = payload.TryGetProperty("document_id", out var documentIdElement)
            && Guid.TryParse(documentIdElement.GetString(), out var parsedDocumentId)
            ? parsedDocumentId
            : Guid.Empty;

        return new DocumentChunk(
            chunkId,
            parsedDocId,
            indexName,
            payload.TryGetProperty("text", out var textElement) ? textElement.GetString() ?? string.Empty : string.Empty,
            payload.TryGetProperty("order", out var orderElement) ? orderElement.GetInt32() : 0,
            payload.TryGetProperty("page_number", out var pageElement) && pageElement.ValueKind == JsonValueKind.Number
                ? pageElement.GetInt32()
                : null,
            payload.TryGetProperty("section", out var sectionElement) ? sectionElement.GetString() : null,
            payload.TryGetProperty("heading", out var headingElement) ? headingElement.GetString() : null,
            payload.TryGetProperty("parent_chunk_id", out var parentElement) && parentElement.ValueKind == JsonValueKind.Number
                ? parentElement.GetInt32()
                : null,
            payload.TryGetProperty("token_count", out var tokenElement) ? tokenElement.GetInt32() : 0,
            metadata);
    }

    private static object? BuildFilter(VectorFilter filter)
    {
        var must = new List<object?>();

        if (!string.IsNullOrEmpty(filter.TenantId))
        {
            must.Add(new { key = "tenant_id", match = new { value = filter.TenantId } });
        }

        if (filter.DocumentIds is { Count: > 0 })
        {
            must.Add(new
            {
                key = "document_id",
                match = new { any = filter.DocumentIds.Select(id => id.ToString()).ToArray() }
            });
        }

        if (filter.Categories is { Count: > 0 })
        {
            must.Add(new
            {
                should = filter.Categories.Select(c => (object)new { key = "metadata.classification", match = new { value = c } })
                    .Concat(filter.Categories.Select(c => (object)new { key = "metadata.documentType", match = new { value = c } }))
                    .ToArray()
            });
        }

        if (filter.Metadata is { Count: > 0 })
        {
            foreach (var (key, value) in filter.Metadata)
            {
                must.Add(new { key = $"metadata.{key}", match = new { value } });
            }
        }

        if (must.Count == 0)
        {
            return null;
        }

        return new { must = must.ToArray() };
    }

    private static float Bm25Score(
        string text,
        IReadOnlyList<string> queryTokens,
        IReadOnlyDictionary<string, int> documentFrequency,
        int totalDocuments,
        double averageLength)
    {
        const float k1 = 1.2f;
        const float b = 0.75f;

        var tokens = VectorMath.Tokenize(text);
        if (tokens.Count == 0)
        {
            return 0f;
        }

        var termFrequency = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var token in tokens)
        {
            termFrequency[token] = termFrequency.TryGetValue(token, out var count) ? count + 1 : 1;
        }

        double score = 0;
        foreach (var token in queryTokens.Distinct())
        {
            if (!termFrequency.TryGetValue(token, out var tf) || tf == 0)
            {
                continue;
            }

            var df = documentFrequency.TryGetValue(token, out var freq) ? freq : 1;
            var idf = Math.Log(1.0 + ((totalDocuments - df + 0.5) / (df + 0.5)));
            var denominator = tf + k1 * (1 - b + b * (tokens.Count / averageLength));
            score += idf * ((tf * (k1 + 1)) / denominator);
        }

        return (float)score;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorBody = await response.Content.ReadAsStringAsync(ct);
        throw new InvalidOperationException(
            $"Qdrant {operation} failed with status {(int)response.StatusCode}: {errorBody}");
    }
}
