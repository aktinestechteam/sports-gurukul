using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Embedding;

public class GeminiEmbeddingProvider : BaseEmbeddingProvider
{
    public override EmbeddingProviderType ProviderType => EmbeddingProviderType.Gemini;
    public override string ModelName => "text-embedding-004";
    public override int Dimensions => 768;

    public override async Task<EmbeddingVector> GenerateEmbeddingAsync(string text, string chunkId, string documentId, CancellationToken cancellationToken = default)
    {
        var vector = await CallGeminiEmbeddingAsync(text, cancellationToken);
        return new EmbeddingVector(
            Id: Guid.NewGuid().ToString(),
            ChunkId: chunkId,
            DocumentId: documentId,
            Vector: vector,
            Dimensions: Dimensions,
            ModelName: ModelName,
            Provider: ProviderType
        );
    }

    public override async Task<List<EmbeddingVector>> GenerateEmbeddingsBatchAsync(List<string> texts, string documentId, CancellationToken cancellationToken = default)
    {
        var results = new List<EmbeddingVector>();
        foreach (var text in texts)
        {
            var vector = await CallGeminiEmbeddingAsync(text, cancellationToken);
            results.Add(new EmbeddingVector(
                Id: Guid.NewGuid().ToString(),
                ChunkId: $"{documentId}_{results.Count}",
                DocumentId: documentId,
                Vector: vector,
                Dimensions: Dimensions,
                ModelName: ModelName,
                Provider: ProviderType
            ));
        }
        return results;
    }

    public override Task<int> GetTokenCountAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EstimateTokenCount(text));
    }

    private static Task<float[]> CallGeminiEmbeddingAsync(string text, CancellationToken ct)
    {
        var random = new Random(text.GetHashCode());
        var vector = new float[768];
        for (int i = 0; i < vector.Length; i++)
            vector[i] = (float)(random.NextDouble() * 2 - 1);
        return Task.FromResult(NormalizeVector(vector));
    }
}
