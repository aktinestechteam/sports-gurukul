using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Configuration;

public class KnowledgePlatformOptions
{
    public const string SectionName = "KnowledgePlatform";

    public EmbeddingOptions Embedding { get; set; } = new();
    public VectorStoreOptions VectorStore { get; set; } = new();
    public ChunkingConfiguration Chunking { get; set; } = new();
    public RetrievalOptions Retrieval { get; set; } = new();
    public SecurityOptions Security { get; set; } = new();
    public ObservabilityOptions Observability { get; set; } = new();
}

public class EmbeddingOptions
{
    public string Provider { get; set; } = "deterministic";
    public int BatchSize { get; set; } = 64;
    public int Dimensions { get; set; } = 384;
    public bool CacheEnabled { get; set; } = true;
    public int CacheCapacity { get; set; } = 10000;
    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
    public string? Model { get; set; }
    public string? DeploymentName { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
}

public class VectorStoreOptions
{
    public string Provider { get; set; } = "inmemory";
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? CollectionPrefix { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
    public bool CreateCollectionIfMissing { get; set; } = true;
    public int ShardNumber { get; set; } = 1;
}

public class ChunkingConfiguration
{
    public ChunkingStrategyType Strategy { get; set; } = ChunkingStrategyType.Recursive;
    public int ChunkSize { get; set; } = 512;
    public int ChunkOverlap { get; set; } = 64;
    public int MinChunkSize { get; set; } = 64;
    public bool UseTokenEstimation { get; set; } = false;
    public int ParentChunkSize { get; set; } = 1024;
    public int ChildChunkSize { get; set; } = 256;
}

public class RetrievalOptions
{
    public SearchMode DefaultMode { get; set; } = SearchMode.Hybrid;
    public int DefaultTopK { get; set; } = 10;
    public float MinScore { get; set; } = 0.0f;
    public string Reranker { get; set; } = "score";
    public float VectorWeight { get; set; } = 0.7f;
    public float KeywordWeight { get; set; } = 0.3f;
    public bool EnableReRanking { get; set; } = true;
}

public class SecurityOptions
{
    public string? EncryptionKeyBase64 { get; set; }
    public bool EnableAudit { get; set; } = true;
    public bool EnforceTenantIsolation { get; set; } = true;
    public int AuditBufferSize { get; set; } = 5000;
}

public class ObservabilityOptions
{
    public int LatencySampleLimit { get; set; } = 1000;
}
