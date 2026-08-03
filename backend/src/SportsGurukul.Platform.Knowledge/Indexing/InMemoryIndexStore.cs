using System.Collections.Concurrent;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Indexing;

internal sealed class InMemoryIndexStore : IKnowledgeIndexStore
{
    private readonly ConcurrentDictionary<string, KnowledgeIndex> _indexes = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<Guid, KnowledgeDocumentRecord> _documents = new();

    public Task<KnowledgeIndex?> GetIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var key = Key(name, tenantId);
        return Task.FromResult(_indexes.TryGetValue(key, out var index) ? index : null);
    }

    public Task<KnowledgeIndex> CreateIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var key = Key(name, tenantId);
        var index = _indexes.GetOrAdd(key, _ => KnowledgeIndex.New(name, tenantId));
        return Task.FromResult(index);
    }

    public Task UpdateIndexAsync(KnowledgeIndex index, CancellationToken ct = default)
    {
        _indexes[Key(index.Name, index.TenantId)] = index;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<KnowledgeIndexSummary>> ListIndexesAsync(string? tenantId = null, CancellationToken ct = default)
    {
        var indexes = _indexes.Values
            .Where(i => tenantId is null || string.Equals(i.TenantId, tenantId, StringComparison.Ordinal))
            .OrderBy(i => i.Name)
            .Select(i => new KnowledgeIndexSummary(
                i.Name,
                i.TenantId,
                i.State,
                i.DocumentCount,
                i.ChunkCount,
                i.Version,
                i.CreatedAtUtc,
                i.LastIndexedAtUtc))
            .ToList();

        return Task.FromResult<IReadOnlyList<KnowledgeIndexSummary>>(indexes);
    }

    public Task DeleteIndexAsync(string name, string tenantId, CancellationToken ct = default)
    {
        var key = Key(name, tenantId);
        _indexes.TryRemove(key, out _);
        foreach (var (id, record) in _documents)
        {
            if (string.Equals(record.IndexName, name, StringComparison.Ordinal)
                && string.Equals(record.TenantId, tenantId, StringComparison.Ordinal))
            {
                _documents.TryRemove(id, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<KnowledgeDocumentRecord?> GetDocumentAsync(Guid documentId, CancellationToken ct = default) =>
        Task.FromResult(_documents.TryGetValue(documentId, out var record) ? record : null);

    public Task SaveDocumentAsync(KnowledgeDocumentRecord record, CancellationToken ct = default)
    {
        _documents[record.DocumentId] = record;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<KnowledgeDocumentRecord>> ListDocumentsAsync(string indexName, string? tenantId = null, CancellationToken ct = default)
    {
        var records = _documents.Values
            .Where(r => string.Equals(r.IndexName, indexName, StringComparison.Ordinal))
            .Where(r => tenantId is null || string.Equals(r.TenantId, tenantId, StringComparison.Ordinal))
            .OrderBy(r => r.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<KnowledgeDocumentRecord>>(records);
    }

    public Task<IReadOnlyList<string>> ListFingerprintsAsync(string indexName, string? tenantId = null, CancellationToken ct = default)
    {
        var fingerprints = _documents.Values
            .Where(r => string.Equals(r.IndexName, indexName, StringComparison.Ordinal))
            .Where(r => tenantId is null || string.Equals(r.TenantId, tenantId, StringComparison.Ordinal))
            .Where(r => !r.IsArchived)
            .Select(r => r.Fingerprint)
            .ToList();

        return Task.FromResult<IReadOnlyList<string>>(fingerprints);
    }

    public Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default)
    {
        _documents.TryRemove(documentId, out _);
        return Task.CompletedTask;
    }

    private static string Key(string name, string tenantId) => $"{tenantId}/{name}";
}
