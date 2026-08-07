using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public interface IEmbeddingProvider
{
    string Name { get; }
    int Dimensions { get; }
    Task<EmbeddingVector> EmbedAsync(string text, CancellationToken ct = default);
    Task<IReadOnlyList<EmbeddingVector>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default);
    Task<bool> IsHealthyAsync(CancellationToken ct = default);
}

public interface IEmbeddingProviderFactory
{
    IEmbeddingProvider GetProvider(string? name = null);
}

public interface IEmbeddingService
{
    IEmbeddingProvider Provider { get; }
    Task<IReadOnlyList<ChunkEmbedding>> EmbedChunksAsync(
        IReadOnlyList<DocumentChunk> chunks,
        string tenantId,
        string ownerUserId,
        CancellationToken ct = default);
    Task<EmbeddingVector> EmbedQueryAsync(string query, CancellationToken ct = default);
}

public record VectorStoreCapabilities(bool SupportsVector, bool SupportsKeyword, bool SupportsMetadataFiltering);

public interface IVectorStore
{
    string Name { get; }
    VectorStoreCapabilities Capabilities { get; }
    Task UpsertAsync(ChunkEmbedding embedding, CancellationToken ct = default);
    Task UpsertBatchAsync(IReadOnlyList<ChunkEmbedding> embeddings, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid chunkId, CancellationToken ct = default);
    Task DeleteBatchAsync(IReadOnlyList<Guid> chunkIds, CancellationToken ct = default);
    Task<int> DeleteByFilterAsync(VectorFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<RetrievedChunk>> SearchAsync(VectorSearchQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<RetrievedChunk>> SearchByTextAsync(KeywordSearchQuery query, CancellationToken ct = default);
    Task<long> CountAsync(string? indexName = null, CancellationToken ct = default);
    Task ResetAsync(string? indexName = null, CancellationToken ct = default);
    Task<bool> IsHealthyAsync(CancellationToken ct = default);
}

public interface IVectorStoreFactory
{
    IVectorStore GetStore(string? name = null);
}
