using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class KnowledgeIngestionService : IKnowledgeIngestionService
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly IChunkingService _chunkingService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreService _vectorStoreService;
    private readonly IDeduplicationService? _deduplicationService;
    private readonly IDocumentFingerprinter? _fingerprinter;
    private readonly ILogger<KnowledgeIngestionService> _logger;

    public KnowledgeIngestionService(
        IDocumentProcessor documentProcessor,
        IChunkingService chunkingService,
        IEmbeddingService embeddingService,
        IVectorStoreService vectorStoreService,
        ILogger<KnowledgeIngestionService> logger,
        IDeduplicationService? deduplicationService = null,
        IDocumentFingerprinter? fingerprinter = null)
    {
        _documentProcessor = documentProcessor;
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
        _vectorStoreService = vectorStoreService;
        _logger = logger;
        _deduplicationService = deduplicationService;
        _fingerprinter = fingerprinter;
    }

    public async Task<ProcessingStatus> IngestDocumentAsync(
        RawDocument document,
        ChunkingOptions chunkingOptions,
        EmbeddingProviderType embeddingProvider,
        string vectorStoreName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting ingestion of document {DocumentId}: {FileName}", document.Id, document.FileName);

        if (_fingerprinter != null && _deduplicationService != null)
        {
            var checksum = await _fingerprinter.ComputeChecksumAsync(document.Content, cancellationToken);
            if (await _deduplicationService.IsDuplicateAsync(document.Id, checksum, cancellationToken))
            {
                _logger.LogWarning("Document {DocumentId} already indexed, skipping", document.Id);
                return ProcessingStatus.Indexed;
            }
        }

        var extracted = await _documentProcessor.ProcessAsync(document, cancellationToken);
        if (string.IsNullOrWhiteSpace(extracted.Text))
        {
            _logger.LogWarning("No text extracted from document {DocumentId}", document.Id);
            return ProcessingStatus.Failed;
        }

        var chunks = await _chunkingService.ChunkDocumentAsync(extracted, chunkingOptions, cancellationToken);
        if (chunks.Count == 0)
        {
            _logger.LogWarning("No chunks generated for document {DocumentId}", document.Id);
            return ProcessingStatus.Failed;
        }

        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(chunks, embeddingProvider, "", cancellationToken);

        var storeType = Enum.TryParse<VectorStoreType>(vectorStoreName, true, out var parsed)
            ? parsed : VectorStoreType.Qdrant;

        var indexName = $"kb_{document.Id[..Math.Min(8, document.Id.Length)]}";
        await _vectorStoreService.StoreEmbeddingsAsync(indexName, embeddings, storeType, cancellationToken);

        if (_fingerprinter != null && _deduplicationService != null)
        {
            var checksum = await _fingerprinter.ComputeChecksumAsync(document.Content, cancellationToken);
            var contentHash = await _fingerprinter.ComputeContentHashAsync(extracted.Text, cancellationToken);
            await _deduplicationService.MarkAsIndexedAsync(document.Id, checksum, contentHash, cancellationToken);
        }

        _logger.LogInformation(
            "Successfully ingested document {DocumentId}: {Chunks} chunks, {Embeddings} embeddings",
            document.Id, chunks.Count, embeddings.Count);

        return ProcessingStatus.Indexed;
    }

    public async Task<ProcessingStatus> IngestDocumentBatchAsync(
        List<RawDocument> documents,
        ChunkingOptions chunkingOptions,
        EmbeddingProviderType embeddingProvider,
        string vectorStoreName,
        CancellationToken cancellationToken = default)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 3,
            CancellationToken = cancellationToken
        };

        var results = new List<ProcessingStatus>();

        await Task.WhenAll(
            documents.Select(doc => Task.Run(async () =>
            {
                var result = await IngestDocumentAsync(doc, chunkingOptions, embeddingProvider, vectorStoreName, cancellationToken);
                lock (results) { results.Add(result); }
            }, cancellationToken))
        );

        var successCount = results.Count(r => r == ProcessingStatus.Indexed);
        _logger.LogInformation("Batch ingestion complete: {Success}/{Total} documents indexed", successCount, documents.Count);

        return successCount == documents.Count ? ProcessingStatus.Indexed : ProcessingStatus.Failed;
    }

    public async Task<bool> DeleteDocumentAsync(
        string documentId,
        string vectorStoreName,
        CancellationToken cancellationToken = default)
    {
        var storeType = Enum.TryParse<VectorStoreType>(vectorStoreName, true, out var parsed)
            ? parsed : VectorStoreType.Qdrant;

        var indexName = $"kb_{documentId[..Math.Min(8, documentId.Length)]}";
        await _vectorStoreService.DeleteDocumentVectorsAsync(indexName, documentId, storeType, cancellationToken);

        _logger.LogInformation("Deleted document {DocumentId} from knowledge store", documentId);
        return true;
    }

    public async Task<ProcessingStatus> ReindexDocumentAsync(
        string documentId,
        ChunkingOptions chunkingOptions,
        EmbeddingProviderType embeddingProvider,
        string vectorStoreName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reindexing document {DocumentId}", documentId);

        await DeleteDocumentAsync(documentId, vectorStoreName, cancellationToken);

        return ProcessingStatus.Pending;
    }
}
