using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Observability;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class ObservabilityTests
{
    [Fact]
    public void MetricsCollector_Tracks_Indexing()
    {
        var metrics = new KnowledgeMetricsCollector(new ObservabilityOptions());

        metrics.RecordDocumentIndexed("sports", 4);
        metrics.RecordDocumentIndexed("sports", 2);
        metrics.RecordDocumentFailed("sports", "boom");

        var index = metrics.GetIndexMetrics("sports");

        Assert.Equal(2, index.DocumentsIndexed);
        Assert.Equal(6, index.ChunksIndexed);
        Assert.Equal(1, index.DocumentsFailed);
        Assert.NotNull(index.LastIndexedAt);
    }

    [Fact]
    public void MetricsCollector_Tracks_Embedding()
    {
        var metrics = new KnowledgeMetricsCollector(new ObservabilityOptions());

        metrics.RecordEmbedding(16, 384, TimeSpan.FromMilliseconds(50));
        metrics.RecordEmbedding(8, 384, TimeSpan.FromMilliseconds(30));

        var embedding = metrics.GetEmbeddingMetrics();

        Assert.Equal(2, embedding.Calls);
        Assert.Equal(24, embedding.TotalVectors);
        Assert.Equal(40, embedding.AverageMs, precision: 1);
    }

    [Fact]
    public void MetricsCollector_Computes_LatencyPercentiles()
    {
        var metrics = new KnowledgeMetricsCollector(new ObservabilityOptions());

        metrics.RecordSearch("sports", SearchMode.Hybrid, 5, 10, 50);
        metrics.RecordSearch("sports", SearchMode.Hybrid, 3, 100, 40);
        metrics.RecordAccessDenied("sports");

        var latency = metrics.GetSearchLatency();
        var retrieval = metrics.GetRetrievalMetrics();

        Assert.Equal(2, latency.Requests);
        Assert.Equal(55, latency.P50Ms, precision: 1);
        Assert.Equal(100, latency.P95Ms, precision: 1);
        Assert.Equal(1, retrieval.AccessDeniedCount);
        Assert.Equal(2, retrieval.Searches);
        Assert.Equal(90, retrieval.TotalCandidates);
    }

    [Fact]
    public async Task HealthService_Reports_Healthy_ForDeterministicAndInMemory()
    {
        using var provider = TestHarness.CreateProvider();
        var health = provider.GetRequiredService<IKnowledgeHealthService>();

        var report = await health.GetHealthAsync();

        Assert.Equal(KnowledgeHealthState.Healthy, report.State);
        Assert.True(report.Components["embedding"].Healthy);
        Assert.True(report.Components["vectorStore"].Healthy);
    }
}
