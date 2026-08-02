using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Mocks;

public class MockEmbeddingProvider : IEmbeddingProvider
{
    public EmbeddingProviderType ProviderType => EmbeddingProviderType.OpenAI;
    public string ModelName => "test-model";
    public int Dimensions => 4;
    public bool SupportsBatchProcessing => true;

    private readonly float[] _fixedVector;

    public MockEmbeddingProvider(float[]? fixedVector = null)
    {
        _fixedVector = fixedVector ?? [0.1f, 0.2f, 0.3f, 0.4f];
    }

    public Task<EmbeddingVector> GenerateEmbeddingAsync(string text, string chunkId, string documentId, CancellationToken cancellationToken = default)
    {
        var vector = GenerateVectorFromText(text);
        return Task.FromResult(new EmbeddingVector(
            Guid.NewGuid().ToString(), chunkId, documentId, vector, Dimensions, ModelName, ProviderType));
    }

    public Task<List<EmbeddingVector>> GenerateEmbeddingsBatchAsync(List<string> texts, string documentId, CancellationToken cancellationToken = default)
    {
        var results = texts.Select((text, i) => new EmbeddingVector(
            Guid.NewGuid().ToString(), $"{documentId}_{i}", documentId,
            GenerateVectorFromText(text), Dimensions, ModelName, ProviderType)).ToList();
        return Task.FromResult(results);
    }

    public Task<int> GetTokenCountAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
    }

    private float[] GenerateVectorFromText(string text) =>
        _fixedVector.Select((v, i) => v + (text.GetHashCode() % 10) * 0.01f * (i + 1)).ToArray();
}
