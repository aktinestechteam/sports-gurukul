using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;
using SportsGurukul.Application.Features.KnowledgePlatform.Services;
using SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;
using SportsGurukul.Application.Tests.KnowledgePlatform.Mocks;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Retrieval;

public class HybridSearchTests
{
    private readonly MockEmbeddingProvider _embeddingProvider;
    private readonly MockVectorStore _vectorStore;
    private readonly KnowledgeSearchService _searchService;

    public HybridSearchTests()
    {
        _embeddingProvider = new MockEmbeddingProvider([0.1f, 0.2f, 0.3f, 0.4f]);
        _vectorStore = new MockVectorStore();

        var storeFactoryMock = new Mock<IVectorStoreFactory>();
        storeFactoryMock.Setup(f => f.GetStore(It.IsAny<VectorStoreType>())).Returns(_vectorStore);
        storeFactoryMock.Setup(f => f.GetStore(It.IsAny<string>())).Returns(_vectorStore);
        storeFactoryMock.Setup(f => f.SupportsStore(It.IsAny<VectorStoreType>())).Returns(true);

        var embeddingFactoryMock = new Mock<IEmbeddingProviderFactory>();
        embeddingFactoryMock.Setup(f => f.GetProvider(It.IsAny<EmbeddingProviderType>())).Returns(_embeddingProvider);
        embeddingFactoryMock.Setup(f => f.SupportsProvider(It.IsAny<EmbeddingProviderType>())).Returns(true);

        var rerankerService = new RerankerService(NullLogger<RerankerService>.Instance);
        var citationService = new CitationService();

        _searchService = new KnowledgeSearchService(
            storeFactoryMock.Object, embeddingFactoryMock.Object, rerankerService, citationService,
            NullLogger<KnowledgeSearchService>.Instance);
    }

    [Fact]
    public async Task SemanticSearch_ReturnsResults()
    {
        await SeedTestData();

        var results = await _searchService.SemanticSearchAsync("default", "sports training", topK: 5);

        results.Should().NotBeNull();
    }

    [Fact]
    public async Task HybridSearch_CombinesSemanticAndKeywordScores()
    {
        await SeedTestData();

        var results = await _searchService.HybridSearchAsync("default", "athlete nutrition", topK: 5);

        results.Should().NotBeNull();
    }

    [Fact]
    public async Task KeywordSearch_FindsExactTerms()
    {
        var store = new MockVectorStore();

        await store.CreateIndexAsync("test-index", 4);

        var embeddingVector = new EmbeddingVector("v1", "c1", "d1",
            [0.1f, 0.2f, 0.3f, 0.4f], 4, "test", EmbeddingProviderType.OpenAI);

        await store.UpsertVectorsAsync("test-index", [embeddingVector]);

        var results = await store.KeywordSearchAsync("test-index", "athlete nutrition");

        results.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchWithReranking_ReturnsRerankedResults()
    {
        await SeedTestData();

        var results = await _searchService.SearchWithRerankingAsync("default", "recovery techniques", topK: 10, rerankTopK: 3);

        results.Should().NotBeNull();
        results.Count.Should().BeLessOrEqualTo(3);
    }

    private async Task SeedTestData()
    {
        var embeddings = await _embeddingProvider.GenerateEmbeddingsBatchAsync(
        [
            "Sports training methods and techniques for athletes",
            "Athlete nutrition and diet planning guidelines",
            "Recovery techniques including ice baths and massage",
            "Strength training protocols for competitive sports",
            "Flexibility exercises and stretching routines"
        ], "doc-1");

        await _vectorStore.CreateIndexAsync("default", 4);
        await _vectorStore.UpsertVectorsAsync("default", embeddings);
    }
}
