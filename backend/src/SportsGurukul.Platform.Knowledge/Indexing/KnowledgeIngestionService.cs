using System.Diagnostics;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Indexing;

internal sealed class KnowledgeIngestionService : IKnowledgeIngestionService
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly IChunkingService _chunkingService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreFactory _vectorStoreFactory;
    private readonly IKnowledgeIndexStore _indexStore;
    private readonly IDeduplicator _deduplicator;
    private readonly IKnowledgeAuditLogger _auditLogger;
    private readonly IKnowledgeMetricsCollector _metrics;
    private readonly KnowledgePlatformOptions _options;

    public KnowledgeIngestionService(
        IDocumentProcessor documentProcessor,
        IChunkingService chunkingService,
        IEmbeddingService embeddingService,
        IVectorStoreFactory vectorStoreFactory,
        IKnowledgeIndexStore indexStore,
        IDeduplicator deduplicator,
        IKnowledgeAuditLogger auditLogger,
        IKnowledgeMetricsCollector metrics,
        KnowledgePlatformOptions options)
    {
        _documentProcessor = documentProcessor;
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
        _vectorStoreFactory = vectorStoreFactory;
        _indexStore = indexStore;
        _deduplicator = deduplicator;
        _auditLogger = auditLogger;
        _metrics = metrics;
        _options = options;
    }

    public async Task<IngestionReport> IngestAsync(
        KnowledgeDocument document,
        ChunkingOptions? options = null,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var indexName = string.IsNullOrWhiteSpace(document.IndexName) ? "default" : document.IndexName;
        var tenantId = document.TenantId ?? string.Empty;

        try
        {
            var index = await _indexStore.GetIndexAsync(indexName, tenantId, ct)
                        ?? await _indexStore.CreateIndexAsync(indexName, tenantId, ct);
            if (index.State == IndexLifecycleState.Archived || index.State == IndexLifecycleState.Deleted)
            {
                return Failed(document, indexName, $"Index '{indexName}' is not active.");
            }

            var processed = await _documentProcessor.ProcessAsync(document, null, ct);
            if (processed.State != DocumentIngestionState.Extracted)
            {
                return Failed(document, indexName, processed.Error ?? "Document processing failed.");
            }

            var fingerprints = await _indexStore.ListFingerprintsAsync(indexName, tenantId, ct);
            var duplicateFingerprint = await _deduplicator.FindDuplicateAsync(
                processed.Fingerprint, indexName, tenantId, fingerprints, ct);
            if (duplicateFingerprint is not null)
            {
                stopwatch.Stop();
                await _auditLogger.LogAsync(CreateAuditEvent(
                    KnowledgeAuditAction.Ingest, document, indexName, succeeded: true,
                    reason: "Duplicate skipped"), ct);

                return new IngestionReport(
                    document.Id, indexName, DocumentIngestionState.DuplicateSkipped, 0,
                    stopwatch.Elapsed, duplicateFingerprint);
            }

            var scopedDocument = document with { IndexName = indexName };
            var resolvedOptions = options ?? ToChunkingOptions(_options.Chunking);
            var chunks = _chunkingService.Chunk(scopedDocument, processed.SafeText, resolvedOptions, ct);
            var enrichedChunks = EnrichChunkMetadata(chunks, scopedDocument, processed);

            var embeddingStopwatch = Stopwatch.StartNew();
            var embeddings = await _embeddingService.EmbedChunksAsync(
                enrichedChunks, tenantId, document.OwnerUserId ?? string.Empty, ct);
            embeddingStopwatch.Stop();
            _metrics.RecordEmbedding(embeddings.Count, _embeddingService.Provider.Dimensions, embeddingStopwatch.Elapsed);

            await _vectorStoreFactory.GetStore().UpsertBatchAsync(embeddings, ct);

            await _indexStore.SaveDocumentAsync(new KnowledgeDocumentRecord(
                document.Id,
                indexName,
                document.Title,
                document.DocumentType,
                processed.Fingerprint.Value,
                processed.Language,
                Version: 1,
                DocumentIngestionState.Indexed,
                chunks.Count,
                IsArchived: false,
                DateTime.UtcNow,
                DateTime.UtcNow,
                tenantId,
                document.OwnerUserId ?? string.Empty,
                BuildRecordMetadata(document, processed)));

            await _indexStore.UpdateIndexAsync(index with
            {
                DocumentCount = index.DocumentCount + 1,
                ChunkCount = index.ChunkCount + chunks.Count,
                LastIndexedAtUtc = DateTime.UtcNow
            }, ct);

            _metrics.RecordDocumentIndexed(indexName, chunks.Count);
            stopwatch.Stop();

            await _auditLogger.LogAsync(CreateAuditEvent(
                KnowledgeAuditAction.Ingest, document, indexName, succeeded: true,
                reason: $"{chunks.Count} chunks indexed"), ct);

            return new IngestionReport(document.Id, indexName, DocumentIngestionState.Indexed, chunks.Count, stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.RecordDocumentFailed(indexName, ex.Message);
            await _auditLogger.LogAsync(CreateAuditEvent(
                KnowledgeAuditAction.Ingest, document, indexName, succeeded: false, reason: ex.Message), ct);

            return new IngestionReport(document.Id, indexName, DocumentIngestionState.Failed, 0, stopwatch.Elapsed, Error: ex.Message);
        }
    }

    public async Task<IReadOnlyList<IngestionReport>> IngestBatchAsync(
        IReadOnlyList<KnowledgeDocument> documents,
        ChunkingOptions? options = null,
        CancellationToken ct = default)
    {
        var reports = new List<IngestionReport>(documents.Count);
        foreach (var document in documents)
        {
            ct.ThrowIfCancellationRequested();
            reports.Add(await IngestAsync(document, options, ct));
        }

        return reports;
    }

    public async Task<bool> DeleteAsync(Guid documentId, CancellationToken ct = default)
    {
        var record = await _indexStore.GetDocumentAsync(documentId, ct);
        if (record is null)
        {
            return false;
        }

        var filter = new VectorFilter(record.IndexName, record.TenantId, new[] { documentId });
        await _vectorStoreFactory.GetStore().DeleteByFilterAsync(filter, ct);
        await _indexStore.DeleteDocumentAsync(documentId, ct);

        var index = await _indexStore.GetIndexAsync(record.IndexName, record.TenantId, ct);
        if (index is not null)
        {
            await _indexStore.UpdateIndexAsync(index with
            {
                DocumentCount = Math.Max(0, index.DocumentCount - 1),
                ChunkCount = Math.Max(0, index.ChunkCount - record.ChunkCount),
                LastIndexedAtUtc = DateTime.UtcNow
            }, ct);
        }

        await _auditLogger.LogAsync(new KnowledgeAuditEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            KnowledgeAuditAction.Delete,
            record.OwnerUserId,
            record.TenantId,
            record.IndexName,
            documentId.ToString(),
            "knowledge",
            true,
            "Document deleted"), ct);

        return true;
    }

    private static IReadOnlyList<DocumentChunk> EnrichChunkMetadata(
        IReadOnlyList<DocumentChunk> chunks,
        KnowledgeDocument document,
        ProcessedDocument processed)
    {
        var enriched = new List<DocumentChunk>(chunks.Count);
        foreach (var chunk in chunks)
        {
            var metadata = new Dictionary<string, string>(chunk.Metadata, StringComparer.Ordinal)
            {
                ["document_id"] = document.Id.ToString(),
                ["document_title"] = document.Title,
                ["classification"] = processed.Classification.Category,
                ["language"] = processed.Language,
                ["index_name"] = document.IndexName ?? "default"
            };
            if (!string.IsNullOrEmpty(document.SourceUri))
            {
                metadata["source_link"] = document.SourceUri;
            }
            else if (!string.IsNullOrEmpty(document.StoragePath))
            {
                metadata["source_link"] = document.StoragePath;
            }

            enriched.Add(chunk with { Metadata = metadata });
        }

        return enriched;
    }

    private static ChunkingOptions ToChunkingOptions(ChunkingConfiguration config) =>
        new(
            config.Strategy,
            config.ChunkSize,
            config.ChunkOverlap,
            config.MinChunkSize,
            config.UseTokenEstimation,
            ParentChunkSize: config.ParentChunkSize,
            ChildChunkSize: config.ChildChunkSize);

    private static IReadOnlyDictionary<string, string> BuildRecordMetadata(
        KnowledgeDocument document,
        ProcessedDocument processed)
    {
        var metadata = new Dictionary<string, string>(processed.Metadata, StringComparer.Ordinal)
        {
            ["contentType"] = document.ContentType ?? string.Empty,
            ["title"] = document.Title,
            ["documentType"] = document.DocumentType.ToString(),
            ["language"] = processed.Language,
            ["classification"] = processed.Classification.Category,
            ["sizeBytes"] = document.SizeBytes.ToString()
        };
        if (!string.IsNullOrEmpty(document.FileName))
        {
            metadata["fileName"] = document.FileName;
        }

        if (!string.IsNullOrEmpty(document.StoragePath))
        {
            metadata["storagePath"] = document.StoragePath;
        }

        if (!string.IsNullOrEmpty(document.SourceUri))
        {
            metadata["sourceUri"] = document.SourceUri;
        }

        return metadata;
    }

    private static KnowledgeAuditEvent CreateAuditEvent(
        KnowledgeAuditAction action,
        KnowledgeDocument document,
        string indexName,
        bool succeeded,
        string? reason) =>
        new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            action,
            document.OwnerUserId ?? string.Empty,
            document.TenantId ?? string.Empty,
            indexName,
            document.Id.ToString(),
            "knowledge",
            succeeded,
            reason);

    private static IngestionReport Failed(KnowledgeDocument document, string indexName, string error) =>
        new(document.Id, indexName, DocumentIngestionState.Failed, 0, TimeSpan.Zero, Error: error);
}
