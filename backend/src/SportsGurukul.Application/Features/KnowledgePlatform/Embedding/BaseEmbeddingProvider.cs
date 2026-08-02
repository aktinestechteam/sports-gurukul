using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Embedding;

public abstract class BaseEmbeddingProvider : IEmbeddingProvider
{
    public abstract EmbeddingProviderType ProviderType { get; }
    public abstract string ModelName { get; }
    public abstract int Dimensions { get; }
    public virtual bool SupportsBatchProcessing => true;

    public abstract Task<EmbeddingVector> GenerateEmbeddingAsync(string text, string chunkId, string documentId, CancellationToken cancellationToken = default);
    public abstract Task<List<EmbeddingVector>> GenerateEmbeddingsBatchAsync(List<string> texts, string documentId, CancellationToken cancellationToken = default);
    public abstract Task<int> GetTokenCountAsync(string text, CancellationToken cancellationToken = default);

    protected static float[] NormalizeVector(float[] vector)
    {
        var magnitude = Math.Sqrt(vector.Sum(v => v * v));
        if (magnitude < float.Epsilon) return vector;
        return vector.Select(v => (float)(v / magnitude)).ToArray();
    }

    protected static int EstimateTokenCount(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length + text.Length / 10;
    }
}
