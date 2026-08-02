using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IKnowledgeSearchService
{
    Task<RetrievalContext> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<RetrievalContext> MultiKnowledgeSearchAsync(List<SearchQuery> queries, RetrievalType mergeStrategy = RetrievalType.Hybrid, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> SemanticSearchAsync(string indexName, string text, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> HybridSearchAsync(string indexName, string text, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> KeywordSearchAsync(string indexName, string query, int topK = 10, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> SearchWithRerankingAsync(string indexName, string text, int topK = 10, int rerankTopK = 5, CancellationToken cancellationToken = default);
}

public interface IKnowledgeIngestionService
{
    Task<ProcessingStatus> IngestDocumentAsync(RawDocument document, ChunkingOptions chunkingOptions, EmbeddingProviderType embeddingProvider, string vectorStoreName, CancellationToken cancellationToken = default);
    Task<ProcessingStatus> IngestDocumentBatchAsync(List<RawDocument> documents, ChunkingOptions chunkingOptions, EmbeddingProviderType embeddingProvider, string vectorStoreName, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentAsync(string documentId, string vectorStoreName, CancellationToken cancellationToken = default);
    Task<ProcessingStatus> ReindexDocumentAsync(string documentId, ChunkingOptions chunkingOptions, EmbeddingProviderType embeddingProvider, string vectorStoreName, CancellationToken cancellationToken = default);
}

public interface IDocumentProcessor
{
    Task<ExtractedDocument> ProcessAsync(RawDocument document, CancellationToken cancellationToken = default);
    Task<ExtractedDocument> ProcessWithExtensionsAsync(RawDocument document, bool enableOcr, bool enableImageCaptioning, bool enablePiiDetection, CancellationToken cancellationToken = default);
}

public interface IChunkingService
{
    Task<List<DocumentChunk>> ChunkDocumentAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default);
}

public interface IEmbeddingService
{
    Task<List<EmbeddingVector>> GenerateEmbeddingsAsync(List<DocumentChunk> chunks, EmbeddingProviderType provider, string modelName, CancellationToken cancellationToken = default);
    Task<BatchEmbeddingResult> GenerateEmbeddingsBatchAsync(BatchEmbeddingRequest request, CancellationToken cancellationToken = default);
}

public interface IVectorStoreService
{
    Task StoreEmbeddingsAsync(string indexName, List<EmbeddingVector> embeddings, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task DeleteDocumentVectorsAsync(string indexName, string documentId, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> SearchAsync(string indexName, float[] queryVector, SearchQuery query, CancellationToken cancellationToken = default);
}

public interface IRetrievalService
{
    Task<RetrievalContext> RetrieveAsync(string indexName, string query, RetrievalType retrievalType, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> RetrieveWithMetadataFilterAsync(string indexName, string query, Dictionary<string, string> filters, CancellationToken cancellationToken = default);
}

public interface IRerankerService
{
    Task<List<RerankingResult>> RerankResultsAsync(string query, List<SearchResult> results, int topK = 10, CancellationToken cancellationToken = default);
}

public interface ICitationService
{
    Citation CreateCitation(SearchResult result);
    List<Citation> CreateCitations(List<SearchResult> results);
    string ToMarkdown(List<Citation> citations);
    string ToJson(List<Citation> citations);
}
