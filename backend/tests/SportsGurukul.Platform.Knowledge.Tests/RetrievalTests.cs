using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Embedding;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Retrieval;
using SportsGurukul.Platform.Knowledge.VectorStores;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class RetrievalTests
{
    [Fact]
    public async Task ScoreReranker_Sorts_ByScoreDescending()
    {
        var reranker = new ScoreReranker();
        var candidates = new[]
        {
            ChunkWithScore(1f, 5, "low"),
            ChunkWithScore(0.9f, 0, "high-rank"),
            ChunkWithScore(0.95f, 3, "mid")
        };

        var ranked = await reranker.RerankAsync("query", candidates);

        Assert.Equal("low", ranked[0].Chunk.Text);
        Assert.Equal("mid", ranked[1].Chunk.Text);
        Assert.Equal(0, ranked[0].Rank);
        Assert.Equal(1, ranked[1].Rank);
    }

    [Fact]
    public async Task RrfReranker_CombinesRankings_AcrossSources()
    {
        var reranker = new RrfReranker();
        var chunkId = Guid.NewGuid();
        var candidates = new[]
        {
            ChunkWithScore(0.5f, 0, "semantic", chunkId, RetrievalStrategy.Semantic),
            ChunkWithScore(0.4f, 1, "keyword", chunkId, RetrievalStrategy.Keyword)
        };

        var ranked = await reranker.RerankAsync("query", candidates);

        Assert.Single(ranked);
        Assert.Equal(chunkId, ranked[0].Chunk.Id);
        Assert.True(ranked[0].Score > 0);
    }

    [Fact]
    public void CitationService_BuildsCitations_FromRetrievedChunks()
    {
        var service = new CitationService();
        var chunk = new DocumentChunk(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "sports",
            "text",
            2,
            PageNumber: 3,
            Heading: "Batting",
            Metadata: new Dictionary<string, string>
            {
                ["document_id"] = "doc-1",
                ["document_title"] = "Coaching Manual",
                ["source_link"] = "https://example.com/manual"
            });

        var citations = service.BuildCitations(new[]
        {
            new RetrievedChunk(chunk, 0.85f, 0, RetrievalStrategy.Semantic)
        });

        var citation = Assert.Single(citations);
        Assert.Equal("Coaching Manual", citation.DocumentName);
        Assert.Equal(3, citation.PageNumber);
        Assert.Equal("Batting", citation.Section);
        Assert.Equal("https://example.com/manual", citation.SourceLink);
    }

    [Fact]
    public async Task InMemoryVectorStore_UpsertAndSearch_FindsMatchingChunk()
    {
        var store = new InMemoryVectorStore();
        var provider = new DeterministicEmbeddingProvider(64);
        var chunk = new DocumentChunk(Guid.NewGuid(), Guid.NewGuid(), "sports", "cricket batting technique", 0);
        var vector = await provider.EmbedAsync(chunk.Text);
        await store.UpsertAsync(new ChunkEmbedding(chunk.Id, "sports", vector, chunk, "t1", "u1"));

        var results = await store.SearchAsync(new VectorSearchQuery(
            await provider.EmbedAsync("cricket batting"),
            5,
            new VectorFilter("sports", "t1")));

        var result = Assert.Single(results);
        Assert.Equal(chunk.Id, result.Chunk.Id);
        Assert.Equal(RetrievalStrategy.Semantic, result.SourceStrategy);
    }

    [Fact]
    public async Task InMemoryVectorStore_KeywordSearch_UsesBm25()
    {
        var store = new InMemoryVectorStore();
        var provider = new DeterministicEmbeddingProvider(16);
        var matching = new DocumentChunk(Guid.NewGuid(), Guid.NewGuid(), "sports", "fast bowling technique", 0);
        var other = new DocumentChunk(Guid.NewGuid(), Guid.NewGuid(), "sports", "nutrition and recovery plan", 1);
        var embeddings = await provider.EmbedBatchAsync(new[] { matching.Text, other.Text });
        await store.UpsertBatchAsync(new[]
        {
            new ChunkEmbedding(matching.Id, "sports", embeddings[0], matching, "t1", "u1"),
            new ChunkEmbedding(other.Id, "sports", embeddings[1], other, "t1", "u1")
        });

        var results = await store.SearchByTextAsync(new KeywordSearchQuery(
            "bowling technique",
            5,
            new VectorFilter("sports", "t1")));

        var top = Assert.Single(results);
        Assert.Equal(matching.Id, top.Chunk.Id);
        Assert.Equal(RetrievalStrategy.Keyword, top.SourceStrategy);
    }

    [Fact]
    public async Task RetrievalService_Hybrid_FusesVectorAndKeyword()
    {
        using var provider = TestHarness.CreateProvider();
        var store = provider.GetRequiredService<IVectorStoreFactory>().GetStore();
        var embedding = provider.GetRequiredService<IEmbeddingService>();
        var chunk = new DocumentChunk(Guid.NewGuid(), Guid.NewGuid(), "sports", "cricket batting drills for juniors", 0);
        var embeddings = await embedding.EmbedChunksAsync(new[] { chunk }, "t1", "u1");
        await store.UpsertBatchAsync(embeddings);

        var retrieval = provider.GetRequiredService<IRetrievalService>();
        var result = await retrieval.SearchAsync(new KnowledgeSearchRequest(
            "cricket batting",
            "sports",
            "t1",
            Mode: SearchMode.Hybrid,
            TopK: 5));

        Assert.NotEmpty(result.Chunks);
        Assert.Equal(chunk.Id, result.Chunks[0].Chunk.Id);
        Assert.True(result.TotalCandidates >= 1);
    }

    [Fact]
    public async Task RetrievalService_Keyword_RespectsCapabilities()
    {
        using var provider = TestHarness.CreateProvider();
        var store = provider.GetRequiredService<IVectorStoreFactory>().GetStore();
        Assert.True(store.Capabilities.SupportsKeyword);

        var embedding = provider.GetRequiredService<IEmbeddingService>();
        var chunk = new DocumentChunk(Guid.NewGuid(), Guid.NewGuid(), "sports", "leg spin wrist position", 0);
        var embeddings = await embedding.EmbedChunksAsync(new[] { chunk }, "t1", "u1");
        await store.UpsertBatchAsync(embeddings);

        var retrieval = provider.GetRequiredService<IRetrievalService>();
        var result = await retrieval.SearchAsync(new KnowledgeSearchRequest(
            "wrist spin",
            "sports",
            "t1",
            Mode: SearchMode.Keyword,
            TopK: 5));

        Assert.NotEmpty(result.Chunks);
    }

    private static RetrievedChunk ChunkWithScore(
        float score,
        int rank,
        string text,
        Guid? id = null,
        RetrievalStrategy strategy = RetrievalStrategy.Semantic) =>
        new(
            new DocumentChunk(id ?? Guid.NewGuid(), Guid.NewGuid(), "sports", text, rank),
            score,
            rank,
            strategy);
}
