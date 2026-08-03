namespace SportsGurukul.Platform.Knowledge.Models;

public record KnowledgeDocument(
    Guid Id,
    string Title,
    string ContentType,
    DocumentType DocumentType,
    string? FileName = null,
    string? StoragePath = null,
    string? SourceUri = null,
    string? Language = null,
    long SizeBytes = 0,
    string TenantId = "",
    string OwnerUserId = "",
    string? IndexName = null,
    IReadOnlyDictionary<string, string>? Metadata = null,
    DateTime CreatedAtUtc = default)
{
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public DateTime CreatedAtUtc { get; init; } = CreatedAtUtc == default ? DateTime.UtcNow : CreatedAtUtc;
}

public record DocumentSection(
    string Heading,
    int Level,
    int StartOffset,
    int EndOffset,
    int PageNumber = 0)
{
    public string? Content { get; init; }
}

public record ExtractedDocumentText(
    string Text,
    IReadOnlyList<DocumentSection> Sections,
    IReadOnlyDictionary<string, string>? Metadata = null);

public record PiiFinding(string Type, string Match, int Offset, int Length, string RedactedReplacement);

public record ContentClassification(string Category, double Confidence, IReadOnlyList<string>? Tags = null);

public record DocumentFingerprint(string Algorithm, string Value);

public record ProcessedDocument(
    KnowledgeDocument Document,
    string NormalizedText,
    IReadOnlyList<DocumentSection> Sections,
    string Language,
    ContentClassification Classification,
    DocumentFingerprint Fingerprint,
    IReadOnlyList<PiiFinding> PiiFindings,
    string SafeText,
    IReadOnlyDictionary<string, string> Metadata,
    DocumentIngestionState State,
    string? Error = null);

public record DocumentChunk(
    Guid Id,
    Guid DocumentId,
    string IndexName,
    string Text,
    int Order,
    int? PageNumber = null,
    string? Section = null,
    string? Heading = null,
    int? ParentChunkId = null,
    int TokenCount = 0,
    IReadOnlyDictionary<string, string>? Metadata = null)
{
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

public record IngestionReport(
    Guid DocumentId,
    string IndexName,
    DocumentIngestionState State,
    int ChunkCount,
    TimeSpan Elapsed,
    string? DeduplicatedAgainst = null,
    string? Error = null);
