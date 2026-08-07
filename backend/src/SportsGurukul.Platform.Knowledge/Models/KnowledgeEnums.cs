namespace SportsGurukul.Platform.Knowledge.Models;

public enum DocumentType
{
    Unknown = 0,
    Pdf,
    Word,
    Excel,
    PowerPoint,
    Markdown,
    Html,
    Text,
    Csv,
    Json,
    Xml,
    Image,
    Other
}

public enum ChunkingStrategyType
{
    FixedSize = 0,
    Semantic,
    HeadingBased,
    SlidingWindow,
    Recursive,
    ParentChild
}

public enum EmbeddingProviderKind
{
    Deterministic = 0,
    OpenAi,
    AzureOpenAi,
    Gemini,
    Cohere,
    SentenceTransformers,
    Ollama,
    Custom
}

public enum VectorStoreKind
{
    InMemory = 0,
    Qdrant,
    AzureAiSearch,
    Pinecone,
    Weaviate,
    Milvus,
    Faiss,
    Chroma,
    PgVector,
    Custom
}

public enum RetrievalStrategy
{
    Semantic = 0,
    Keyword,
    Hybrid
}

public enum SearchMode
{
    Vector = 0,
    Keyword,
    Hybrid
}

public enum IndexLifecycleState
{
    Active = 0,
    Archiving,
    Archived,
    Restoring,
    Deleted
}

public enum DocumentIngestionState
{
    Pending = 0,
    Processing,
    Extracted,
    Chunked,
    Embedded,
    Indexed,
    Failed,
    DuplicateSkipped
}

public enum AccessPermission
{
    None = 0,
    Read,
    Write,
    Admin
}

public enum AccessScopeType
{
    Public = 0,
    Authenticated,
    RoleBased,
    OwnerOnly,
    Restricted
}

public enum KnowledgeAuditAction
{
    Ingest = 0,
    Update,
    Delete,
    Archive,
    Restore,
    Reindex,
    Search,
    AccessDenied,
    Export
}

public enum KnowledgeHealthState
{
    Healthy = 0,
    Degraded,
    Unhealthy
}
