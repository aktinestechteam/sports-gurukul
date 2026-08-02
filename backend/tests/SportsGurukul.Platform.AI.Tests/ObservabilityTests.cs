using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Observability;

namespace SportsGurukul.Platform.AI.Tests;

public class ObservabilityTests
{
    [Fact]
    public void Collector_RecordsAgentMetrics()
    {
        var collector = new InMemoryMetricsCollector();

        collector.RecordAgent("coach", 100, 0.01m, new ModelUsage { PromptTokens = 10, CompletionTokens = 5 }, success: true);
        collector.RecordAgent("coach", 200, 0.02m, new ModelUsage { PromptTokens = 20, CompletionTokens = 10 }, success: false, failure: "timeout");

        var snapshot = collector.Snapshot();

        var metric = Assert.Single(snapshot.Agents);
        Assert.Equal("coach", metric.AgentId);
        Assert.Equal(2, metric.TotalCalls);
        Assert.Equal(1, metric.Successful);
        Assert.Equal(1, metric.Failed);
        Assert.Equal(150, metric.AvgLatencyMs);
        Assert.Equal(200, metric.MaxLatencyMs);
        Assert.Equal(0.03m, metric.TotalCost);
        Assert.Equal(45, metric.TotalTokens);
    }

    [Fact]
    public void Collector_RecordsToolAndWorkflowMetrics()
    {
        var collector = new InMemoryMetricsCollector();

        collector.RecordTool("database", "coach", 50, true);
        collector.RecordTool("database", "coach", 60, false, denied: true);

        var snapshot = collector.Snapshot();

        var tool = Assert.Single(snapshot.Tools);
        Assert.Equal(2, tool.TotalCalls);
        Assert.Equal(1, tool.Successful);
        Assert.Equal(0, tool.Failed);
        Assert.Equal(1, tool.Denied);

        collector.RecordWorkflow("onboarding", 5000, true);
        var snapshot2 = collector.Snapshot();
        Assert.Single(snapshot2.Workflows);
        Assert.Equal(5000, snapshot2.Workflows[0].TotalDurationMs);
    }

    [Fact]
    public void Collector_RecordsModelMetrics()
    {
        var collector = new InMemoryMetricsCollector();

        collector.RecordModel("gpt-model", "stub", 300, 0.005m, new ModelUsage { PromptTokens = 100, CompletionTokens = 50 }, true);

        var snapshot = collector.Snapshot();

        var model = Assert.Single(snapshot.Models);
        Assert.Equal("gpt-model", model.Model);
        Assert.Equal(150, model.TotalTokens);
        Assert.Equal(0.005m, model.TotalCost);
    }

    [Fact]
    public async Task ObservabilityService_ReportsHealthyWithLowFailures()
    {
        var collector = new InMemoryMetricsCollector();
        collector.RecordAgent("coach", 100, 0.01m, null, true);
        collector.RecordAgent("coach", 100, 0.01m, null, true);
        collector.RecordAgent("coach", 100, 0.01m, null, true);
        collector.RecordAgent("coach", 100, 0.01m, null, false, "boom");
        var service = new ObservabilityService(collector, collector);

        Assert.True(await service.IsHealthyAsync());
        var snapshot = await service.GetSnapshotAsync();
        Assert.NotNull(snapshot);
    }

    [Fact]
    public async Task ObservabilityService_ReportsUnhealthyWithHighFailures()
    {
        var collector = new InMemoryMetricsCollector();
        for (var i = 0; i < 10; i++)
        {
            collector.RecordAgent("coach", 100, 0.01m, null, i % 2 == 0);
        }

        var service = new ObservabilityService(collector, collector);

        Assert.False(await service.IsHealthyAsync());
    }

    [Fact]
    public async Task Collector_ThreadSafeConcurrentRecording()
    {
        var collector = new InMemoryMetricsCollector();

        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() => collector.RecordTool($"tool-{i % 5}", "coach", i, true)));
        await Task.WhenAll(tasks);

        var snapshot = collector.Snapshot();
        Assert.Equal(5, snapshot.Tools.Count);
        Assert.Equal(100, snapshot.Tools.Sum(t => t.TotalCalls));
    }
}
