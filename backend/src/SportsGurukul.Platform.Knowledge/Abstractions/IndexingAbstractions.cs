using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public interface IKnowledgeIndexStore
{
    Task<KnowledgeIndex?> GetIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task<KnowledgeIndex> CreateIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task UpdateIndexAsync(KnowledgeIndex index, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeIndexSummary>> ListIndexesAsync(string? tenantId = null, CancellationToken ct = default);
    Task DeleteIndexAsync(string name, string tenantId, CancellationToken ct = default);

    Task<KnowledgeDocumentRecord?> GetDocumentAsync(Guid documentId, CancellationToken ct = default);
    Task SaveDocumentAsync(KnowledgeDocumentRecord record, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeDocumentRecord>> ListDocumentsAsync(string indexName, string? tenantId = null, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListFingerprintsAsync(string indexName, string? tenantId = null, CancellationToken ct = default);
    Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default);
}

public interface IKnowledgeIndexService
{
    Task<KnowledgeIndex> CreateIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task DeleteIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task ArchiveIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task RestoreIndexAsync(string name, string tenantId, CancellationToken ct = default);
    Task<IncrementalIndexResult> IncrementalIndexAsync(
        IReadOnlyList<KnowledgeDocument> documents,
        string indexName,
        string tenantId,
        CancellationToken ct = default);
    Task<KnowledgeIndex> ReindexAsync(string indexName, string tenantId, CancellationToken ct = default);
}

public interface IKnowledgeIngestionService
{
    Task<IngestionReport> IngestAsync(KnowledgeDocument document, ChunkingOptions? options = null, CancellationToken ct = default);
    Task<IReadOnlyList<IngestionReport>> IngestBatchAsync(IReadOnlyList<KnowledgeDocument> documents, ChunkingOptions? options = null, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid documentId, CancellationToken ct = default);
}
