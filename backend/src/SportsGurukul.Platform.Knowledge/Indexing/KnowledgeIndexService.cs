using System.Diagnostics;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Indexing;

internal sealed class KnowledgeIndexService : IKnowledgeIndexService
{
    private readonly IKnowledgeIndexStore _indexStore;
    private readonly IKnowledgeIngestionService _ingestionService;
    private readonly IVectorStoreFactory _vectorStoreFactory;
    private readonly IKnowledgeAuditLogger _auditLogger;
    private readonly KnowledgePlatformOptions _options;

    public KnowledgeIndexService(
        IKnowledgeIndexStore indexStore,
        IKnowledgeIngestionService ingestionService,
        IVectorStoreFactory vectorStoreFactory,
        IKnowledgeAuditLogger auditLogger,
        KnowledgePlatformOptions options)
    {
        _indexStore = indexStore;
        _ingestionService = ingestionService;
        _vectorStoreFactory = vectorStoreFactory;
        _auditLogger = auditLogger;
        _options = options;
    }

    public async Task<KnowledgeIndex> CreateIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var existing = await _indexStore.GetIndexAsync(name, tenantId, ct);
        if (existing is not null)
        {
            return existing;
        }

        var index = await _indexStore.CreateIndexAsync(name, tenantId, ct);
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Ingest, tenantId, name, "index", name, true, "Index created"), ct);
        return index;
    }

    public async Task DeleteIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        await _vectorStoreFactory.GetStore().ResetAsync(name, ct);
        await _indexStore.DeleteIndexAsync(name, tenantId, ct);
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Delete, tenantId, name, "index", name, true, "Index deleted"), ct);
    }

    public async Task ArchiveIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var index = await RequireIndexAsync(name, tenantId, ct);
        await _indexStore.UpdateIndexAsync(index with { State = IndexLifecycleState.Archived }, ct);
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Archive, tenantId, name, "index", name, true, "Index archived"), ct);
    }

    public async Task RestoreIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var index = await RequireIndexAsync(name, tenantId, ct);
        await _indexStore.UpdateIndexAsync(index with { State = IndexLifecycleState.Active }, ct);
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Restore, tenantId, name, "index", name, true, "Index restored"), ct);
    }

    public async Task<IncrementalIndexResult> IncrementalIndexAsync(
        IReadOnlyList<KnowledgeDocument> documents,
        string indexName,
        string tenantId,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var added = 0;
        var updated = 0;
        var skipped = 0;

        foreach (var document in documents)
        {
            ct.ThrowIfCancellationRequested();
            var scoped = document with { IndexName = indexName, TenantId = tenantId };
            var existing = await _indexStore.GetDocumentAsync(document.Id, ct);

            var report = await _ingestionService.IngestAsync(scoped, null, ct);
            switch (report.State)
            {
                case DocumentIngestionState.Indexed when existing is null:
                    added++;
                    break;
                case DocumentIngestionState.Indexed:
                    updated++;
                    break;
                case DocumentIngestionState.DuplicateSkipped:
                    skipped++;
                    break;
            }
        }

        stopwatch.Stop();
        return new IncrementalIndexResult(indexName, added, updated, skipped, stopwatch.Elapsed);
    }

    public async Task<KnowledgeIndex> ReindexAsync(string indexName, string tenantId, CancellationToken ct = default)
    {
        var index = await RequireIndexAsync(indexName, tenantId, ct);
        var records = await _indexStore.ListDocumentsAsync(indexName, tenantId, ct);

        await _vectorStoreFactory.GetStore().ResetAsync(indexName, ct);

        foreach (var record in records)
        {
            ct.ThrowIfCancellationRequested();
            await _indexStore.DeleteDocumentAsync(record.DocumentId, ct);
        }

        await _indexStore.UpdateIndexAsync(index with
        {
            DocumentCount = 0,
            ChunkCount = 0,
            LastIndexedAtUtc = null
        }, ct);

        foreach (var record in records)
        {
            ct.ThrowIfCancellationRequested();
            var document = ReconstructDocument(record);
            await _ingestionService.IngestAsync(document, null, ct);
        }

        var refreshed = await RequireIndexAsync(indexName, tenantId, ct);
        await _indexStore.UpdateIndexAsync(refreshed with { Version = refreshed.Version + 1 }, ct);
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Reindex, tenantId, indexName, "index", indexName, true,
            $"{records.Count} documents reindexed"), ct);

        return refreshed with { Version = refreshed.Version + 1 };
    }

    private async Task<KnowledgeIndex> RequireIndexAsync(string name, string tenantId, CancellationToken ct)
    {
        var index = await _indexStore.GetIndexAsync(name, tenantId, ct)
                    ?? await _indexStore.CreateIndexAsync(name, tenantId, ct);
        return index;
    }

    private static KnowledgeDocument ReconstructDocument(KnowledgeDocumentRecord record)
    {
        var metadata = record.Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var contentType = metadata.TryGetValue("contentType", out var ct) ? ct : string.Empty;
        var fileName = metadata.TryGetValue("fileName", out var fn) ? fn : null;
        var storagePath = metadata.TryGetValue("storagePath", out var sp) ? sp : null;
        var sourceUri = metadata.TryGetValue("sourceUri", out var su) ? su : null;
        long.TryParse(metadata.TryGetValue("sizeBytes", out var sb) ? sb : string.Empty, out var sizeBytes);

        return new KnowledgeDocument(
            record.DocumentId,
            record.Title,
            contentType,
            record.DocumentType,
            fileName,
            storagePath,
            sourceUri,
            record.Language,
            sizeBytes,
            record.TenantId,
            record.OwnerUserId,
            record.IndexName,
            metadata);
    }

    private static KnowledgeAuditEvent CreateAuditEvent(
        KnowledgeAuditAction action,
        string tenantId,
        string indexName,
        string entityType,
        string? entityId,
        bool succeeded,
        string? reason) =>
        new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            action,
            string.Empty,
            tenantId,
            indexName,
            entityId,
            entityType,
            succeeded,
            reason);
}
