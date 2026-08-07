namespace SportsGurukul.Platform.Knowledge.Models;

public record KnowledgeIndex(
    string Name,
    string TenantId,
    IndexLifecycleState State,
    int DocumentCount,
    long ChunkCount,
    DateTime CreatedAtUtc,
    DateTime? LastIndexedAtUtc,
    int Version)
{
    public static KnowledgeIndex New(string name, string tenantId) =>
        new(name, tenantId, IndexLifecycleState.Active, 0, 0, DateTime.UtcNow, null, 1);
}

public record KnowledgeDocumentRecord(
    Guid DocumentId,
    string IndexName,
    string Title,
    DocumentType DocumentType,
    string Fingerprint,
    string Language,
    int Version,
    DocumentIngestionState State,
    int ChunkCount,
    bool IsArchived,
    DateTime CreatedAtUtc,
    DateTime? LastUpdatedAtUtc,
    string TenantId,
    string OwnerUserId,
    IReadOnlyDictionary<string, string>? Metadata = null)
{
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

public record KnowledgeIndexSummary(
    string Name,
    string TenantId,
    IndexLifecycleState State,
    int DocumentCount,
    long ChunkCount,
    int Version,
    DateTime CreatedAtUtc,
    DateTime? LastIndexedAtUtc);

public record IncrementalIndexResult(
    string IndexName,
    int AddedDocuments,
    int UpdatedDocuments,
    int SkippedDuplicates,
    TimeSpan Elapsed);
