namespace SportsGurukul.Application.Features.KnowledgePlatform.Models;

public enum DocumentFormat
{
    Pdf,
    Word,
    Excel,
    PowerPoint,
    Markdown,
    Html,
    PlainText,
    Csv,
    Json,
    Xml
}

public enum ChunkingStrategyType
{
    FixedSize,
    Semantic,
    HeadingBased,
    SlidingWindow,
    Recursive,
    ParentChild
}

public enum EmbeddingProviderType
{
    OpenAI,
    AzureOpenAI,
    Gemini,
    Cohere,
    SentenceTransformers,
    Ollama
}

public enum VectorStoreType
{
    Qdrant,
    AzureAISearch,
    Pinecone,
    Weaviate,
    Milvus,
    Faiss,
    Chroma,
    PgVector
}

public enum RetrievalType
{
    Semantic,
    Hybrid,
    Keyword,
    MetadataFiltered
}

public enum ProcessingStatus
{
    Pending,
    Extracting,
    Extracted,
    Chunking,
    Chunked,
    Embedding,
    Embedded,
    Indexed,
    Failed
}

public enum IndexOperation
{
    Create,
    Update,
    Delete,
    Rebuild,
    Incremental
}

public record RawDocument(
    string Id,
    string FileName,
    DocumentFormat Format,
    byte[] Content,
    long FileSizeBytes,
    string? ContentType,
    string? SourceUri,
    Dictionary<string, string>? Metadata
);

public record ExtractedDocument(
    string Id,
    string FileName,
    DocumentFormat Format,
    string Text,
    string? Title,
    int? PageCount,
    string? DetectedLanguage,
    string? Author,
    DateTime? CreatedDate,
    DateTime? ModifiedDate,
    Dictionary<string, string>? Metadata,
    List<ExtractedImage>? Images,
    ProcessingStatus Status
);

public record ExtractedImage(
    int Page,
    byte[]? Data,
    string? Caption,
    string? Format
);

public record DocumentChunk(
    string Id,
    string DocumentId,
    int ChunkIndex,
    string Content,
    int? TokenCount,
    int? CharacterCount,
    string? Heading,
    int? PageNumber,
    int? ParentChunkId,
    Dictionary<string, string>? Metadata,
    ChunkingStrategyType Strategy
);

public record EmbeddingVector(
    string Id,
    string ChunkId,
    string DocumentId,
    float[] Vector,
    int Dimensions,
    string ModelName,
    EmbeddingProviderType Provider
);

public record SearchQuery(
    string Text,
    RetrievalType RetrievalType,
    double? ScoreThreshold = null,
    Dictionary<string, string>? MetadataFilters = null,
    string? VectorStoreName = null,
    string? EmbeddingModel = null,
    int TopK = 10
);

public record SearchResult(
    string DocumentId,
    string ChunkId,
    string Content,
    double Score,
    string DocumentName,
    DocumentFormat Format,
    int? PageNumber,
    string? Section,
    Dictionary<string, string>? Metadata,
    Citation Citation
);

public record Citation(
    string DocumentName,
    string? Section,
    int? PageNumber,
    string ChunkId,
    double Confidence,
    string? SourceLink,
    string? Excerpt
);

public record RerankingResult(
    string DocumentId,
    string ChunkId,
    string Content,
    double OriginalScore,
    double RerankedScore
);

public record KnowledgeIndexInfo(
    string IndexName,
    VectorStoreType StoreType,
    int TotalDocuments,
    int TotalChunks,
    long TotalSizeBytes,
    IndexOperation LastOperation,
    DateTime LastIndexedAt,
    ProcessingStatus Status
);

public record DocumentFingerprint(
    string DocumentId,
    string Checksum,
    string? ContentHash,
    int ContentLength,
    DateTime IndexedAt
);

public record BatchEmbeddingRequest(
    List<DocumentChunk> Chunks,
    EmbeddingProviderType Provider,
    string ModelName
);

public record BatchEmbeddingResult(
    List<EmbeddingVector> Embeddings,
    int TotalTokens,
    long DurationMs
);

public record RetrievalContext(
    List<SearchResult> Results,
    string OriginalQuery,
    string? ExpandedQuery,
    long DurationMs,
    int TotalResults
);

public record KnowledgeAccessPolicy(
    string KnowledgeBaseId,
    List<string> AllowedRoles,
    List<string> AllowedUsers,
    bool IsPublic,
    bool AllowExternalAccess
);
