namespace SportsGurukul.Platform.Knowledge.Models;

public record VectorFilter(
    string IndexName,
    string? TenantId = null,
    IReadOnlyList<Guid>? DocumentIds = null,
    IReadOnlyList<string>? Categories = null,
    IReadOnlyDictionary<string, string>? Metadata = null);

public record VectorSearchQuery(
    EmbeddingVector Vector,
    int TopK,
    VectorFilter Filter,
    float MinScore = 0.0f,
    IReadOnlyList<Guid>? ExcludeChunkIds = null);

public record KeywordSearchQuery(
    string QueryText,
    int TopK,
    VectorFilter Filter,
    float MinScore = 0.0f,
    IReadOnlyList<Guid>? ExcludeChunkIds = null);

public record RetrievedChunk(
    DocumentChunk Chunk,
    float Score,
    int Rank,
    RetrievalStrategy SourceStrategy)
{
    public string? DocumentId => Chunk.Metadata.TryGetValue("document_id", out var id) ? id : null;
    public string? DocumentName => Chunk.Metadata.TryGetValue("document_title", out var name) ? name : null;
    public string? SourceLink => Chunk.Metadata.TryGetValue("source_link", out var link) ? link : null;
}

public record SearchResult(
    IReadOnlyList<RetrievedChunk> Chunks,
    SearchMode Mode,
    int TotalCandidates,
    long ElapsedMs,
    IReadOnlyList<Citation>? Citations = null);

public record KnowledgeSearchRequest(
    string Query,
    string IndexName,
    string TenantId = "",
    string ActorUserId = "",
    IReadOnlyList<string>? Roles = null,
    SearchMode Mode = SearchMode.Hybrid,
    int TopK = 10,
    float MinScore = 0.0f,
    IReadOnlyList<string>? Categories = null,
    IReadOnlyDictionary<string, string>? MetadataFilter = null,
    bool IncludeCitations = true);

public record KnowledgeSearchResponse(
    string Query,
    string IndexName,
    SearchMode Mode,
    long ElapsedMs,
    int TotalCandidates,
    IReadOnlyList<RetrievedChunk> Chunks,
    IReadOnlyList<Citation> Citations);

public record MultiKnowledgeSearchRequest(
    string Query,
    IReadOnlyList<string> IndexNames,
    string TenantId = "",
    string ActorUserId = "",
    IReadOnlyList<string>? Roles = null,
    SearchMode Mode = SearchMode.Hybrid,
    int TopKPerIndex = 5,
    int FinalTopK = 10,
    float MinScore = 0.0f,
    bool IncludeCitations = true);

public record Citation(
    string DocumentName,
    string? Section,
    int? PageNumber,
    Guid ChunkId,
    float Confidence,
    string? SourceLink,
    string? DocumentId = null,
    int? ChunkOrder = null);
