namespace SportsGurukul.Platform.Knowledge.Models;

public record EmbeddingVector(
    float[] Values,
    int Dimensions)
{
    public static EmbeddingVector Empty => new(Array.Empty<float>(), 0);
}

public record ChunkEmbedding(
    Guid ChunkId,
    string IndexName,
    EmbeddingVector Vector,
    DocumentChunk Chunk,
    string TenantId = "",
    string OwnerUserId = "");

public record EmbeddingBatchResult(
    IReadOnlyList<ChunkEmbedding> Embeddings,
    int TotalTokens,
    TimeSpan Elapsed);
