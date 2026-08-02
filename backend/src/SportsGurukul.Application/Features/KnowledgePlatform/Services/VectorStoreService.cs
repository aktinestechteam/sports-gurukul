using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class VectorStoreService : IVectorStoreService
{
    private readonly IVectorStoreFactory _storeFactory;
    private readonly ILogger<VectorStoreService> _logger;

    public VectorStoreService(
        IVectorStoreFactory storeFactory,
        ILogger<VectorStoreService> logger)
    {
        _storeFactory = storeFactory;
        _logger = logger;
    }

    public async Task StoreEmbeddingsAsync(
        string indexName,
        List<EmbeddingVector> embeddings,
        VectorStoreType storeType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Storing {Count} embeddings in index '{Index}' ({Store})",
            embeddings.Count, indexName, storeType);

        var store = _storeFactory.GetStore(storeType);

        if (!await store.IndexExistsAsync(indexName, cancellationToken))
        {
            var dimensions = embeddings.FirstOrDefault()?.Dimensions ?? 1536;
            await store.CreateIndexAsync(indexName, dimensions, null, cancellationToken);
            _logger.LogInformation("Created index '{Index}' with {Dimensions} dimensions", indexName, dimensions);
        }

        await store.UpsertVectorsAsync(indexName, embeddings, cancellationToken);
        _logger.LogInformation("Stored {Count} vectors in index '{Index}'", embeddings.Count, indexName);
    }

    public async Task DeleteDocumentVectorsAsync(
        string indexName,
        string documentId,
        VectorStoreType storeType,
        CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(storeType);
        await store.DeleteVectorsByDocumentAsync(indexName, documentId, cancellationToken);
        _logger.LogInformation("Deleted vectors for document {DocumentId} from index '{Index}'", documentId, indexName);
    }

    public async Task<List<SearchResult>> SearchAsync(
        string indexName,
        float[] queryVector,
        SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(query.VectorStoreName ?? VectorStoreType.Qdrant.ToString());

        return query.RetrievalType switch
        {
            RetrievalType.Semantic => await store.SemanticSearchAsync(
                indexName, queryVector, query.TopK, query.ScoreThreshold, query.MetadataFilters, cancellationToken),

            RetrievalType.Hybrid => await store.HybridSearchAsync(
                indexName, queryVector, query.Text, query.TopK, query.ScoreThreshold, query.MetadataFilters, cancellationToken: cancellationToken),

            RetrievalType.Keyword => await store.KeywordSearchAsync(
                indexName, query.Text, query.TopK, query.MetadataFilters, cancellationToken),

            _ => await store.SemanticSearchAsync(
                indexName, queryVector, query.TopK, query.ScoreThreshold, query.MetadataFilters, cancellationToken)
        };
    }
}
