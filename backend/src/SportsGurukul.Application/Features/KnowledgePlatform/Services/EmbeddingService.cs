using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingProviderFactory _providerFactory;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IEmbeddingProviderFactory providerFactory,
        ILogger<EmbeddingService> logger)
    {
        _providerFactory = providerFactory;
        _logger = logger;
    }

    public async Task<List<EmbeddingVector>> GenerateEmbeddingsAsync(
        List<DocumentChunk> chunks,
        EmbeddingProviderType provider,
        string modelName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating embeddings for {ChunkCount} chunks using {Provider}", chunks.Count, provider);

        var embeddingProvider = _providerFactory.GetProvider(provider);
        var results = new List<EmbeddingVector>();

        if (embeddingProvider.SupportsBatchProcessing && chunks.Count > 1)
        {
            var batchResult = await GenerateEmbeddingsBatchAsync(
                new BatchEmbeddingRequest(chunks, provider, modelName),
                cancellationToken);
            results = batchResult.Embeddings;
        }
        else
        {
            foreach (var chunk in chunks)
            {
                var vector = await embeddingProvider.GenerateEmbeddingAsync(
                    chunk.Content, chunk.Id, chunk.DocumentId, cancellationToken);
                results.Add(vector);
            }
        }

        _logger.LogInformation("Generated {Count} embedding vectors", results.Count);
        return results;
    }

    public async Task<BatchEmbeddingResult> GenerateEmbeddingsBatchAsync(
        BatchEmbeddingRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var embeddingProvider = _providerFactory.GetProvider(request.Provider);
        var texts = request.Chunks.Select(c => c.Content).ToList();
        var vectors = await embeddingProvider.GenerateEmbeddingsBatchAsync(texts, "", cancellationToken);
        stopwatch.Stop();

        var totalTokens = 0;
        foreach (var chunk in request.Chunks)
            totalTokens += await embeddingProvider.GetTokenCountAsync(chunk.Content, cancellationToken);

        return new BatchEmbeddingResult(vectors, totalTokens, stopwatch.ElapsedMilliseconds);
    }
}
