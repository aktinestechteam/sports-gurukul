using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class SearchTests
{
    [Fact]
    public async Task KnowledgeSearchService_Search_ReturnsChunksAndCitations()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var search = provider.GetRequiredService<IKnowledgeSearchService>();
        await ingestion.IngestAsync(TestHarness.Document("Manual", "cricket batting grip and stance", "t1", "sports"));

        var response = await search.SearchAsync(new KnowledgeSearchRequest(
            "cricket batting",
            "sports",
            "t1",
            "user-1",
            new[] { "coach" },
            SearchMode.Hybrid,
            5));

        Assert.NotEmpty(response.Chunks);
        Assert.NotEmpty(response.Citations);
        Assert.Equal("sports", response.IndexName);
    }

    [Fact]
    public async Task KnowledgeSearchService_AnonymousRequest_IsAllowed_AsPublic()
    {
        using var provider = TestHarness.CreateProvider();
        var search = provider.GetRequiredService<IKnowledgeSearchService>();

        var response = await search.SearchAsync(new KnowledgeSearchRequest(
            "anything",
            "sports",
            "t1",
            Mode: SearchMode.Vector,
            TopK: 5));

        Assert.Empty(response.Chunks);
        Assert.Equal(SearchMode.Vector, response.Mode);
    }

    [Fact]
    public async Task KnowledgeSearchService_Enforces_TenantIsolation()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var search = provider.GetRequiredService<IKnowledgeSearchService>();

        await ingestion.IngestAsync(TestHarness.Document("TenantA", "secret batting plan for A", "tenant-a", "sports"));
        await ingestion.IngestAsync(TestHarness.Document("TenantB", "secret batting plan for B", "tenant-b", "sports"));

        var response = await search.SearchAsync(new KnowledgeSearchRequest(
            "batting plan",
            "sports",
            "tenant-a",
            "user-a",
            new[] { "coach" },
            SearchMode.Hybrid,
            5));

        Assert.NotEmpty(response.Chunks);
        Assert.All(response.Chunks, c =>
            Assert.Equal("TenantA", c.Chunk.Metadata["document_title"]));
    }

    [Fact]
    public async Task KnowledgeSearchService_MultiIndex_MergesResults()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var search = provider.GetRequiredService<IKnowledgeSearchService>();

        await ingestion.IngestAsync(TestHarness.Document("DocA", "football passing drills", "t1", "football"));
        await ingestion.IngestAsync(TestHarness.Document("DocB", "cricket fielding drills", "t1", "cricket"));

        var response = await search.SearchMultiKnowledgeAsync(new MultiKnowledgeSearchRequest(
            "drills",
            new[] { "football", "cricket" },
            "t1",
            "user-1",
            new[] { "coach" },
            SearchMode.Hybrid,
            TopKPerIndex: 5,
            FinalTopK: 10));

        Assert.NotEmpty(response.Chunks);
        Assert.Contains(response.Citations, c => c.DocumentName == "DocA");
        Assert.Contains(response.Citations, c => c.DocumentName == "DocB");
    }

    [Fact]
    public async Task Retrieval_ReportsMetrics_AndAuditsSearch()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var search = provider.GetRequiredService<IKnowledgeSearchService>();
        var metrics = provider.GetRequiredService<IKnowledgeMetricsCollector>();

        await ingestion.IngestAsync(TestHarness.Document("Doc", "strength training routine", "t1", "fitness"));
        await search.SearchAsync(new KnowledgeSearchRequest("strength", "fitness", "t1", "user-1", Mode: SearchMode.Hybrid, TopK: 5));

        var retrievalMetrics = metrics.GetRetrievalMetrics();
        Assert.True(retrievalMetrics.Searches >= 1);
        Assert.True(retrievalMetrics.TotalResults >= 1);

        var indexMetrics = metrics.GetIndexMetrics("fitness");
        Assert.True(indexMetrics.DocumentsIndexed >= 1);
    }
}
