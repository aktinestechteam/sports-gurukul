using System.Collections.Concurrent;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Observability;

public class InMemoryMetricsCollector : IMetricsCollector, IMetricsReporter
{
    private readonly ConcurrentDictionary<string, AgentMetric> _agents = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ToolMetric> _tools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, WorkflowMetric> _workflows = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ModelMetric> _models = new(StringComparer.OrdinalIgnoreCase);

    public void RecordAgent(string agentId, long latencyMs, decimal cost, ModelUsage? usage, bool success, string? failure = null)
    {
        var metric = _agents.GetOrAdd(agentId, key => new AgentMetric { AgentId = key });
        lock (metric)
        {
            metric.TotalCalls++;
            if (success)
            {
                metric.Successful++;
            }
            else
            {
                metric.Failed++;
            }

            metric.TotalLatencyMs += latencyMs;
            metric.MaxLatencyMs = Math.Max(metric.MaxLatencyMs, latencyMs);
            metric.AvgLatencyMs = metric.TotalCalls > 0 ? (double)metric.TotalLatencyMs / metric.TotalCalls : 0;
            metric.TotalCost += cost;
            if (usage is not null)
            {
                metric.TotalTokens += usage.TotalTokens;
            }
        }
    }

    public void RecordTool(string toolName, string? agentId, long latencyMs, bool success, string? failure = null, bool denied = false)
    {
        var metric = _tools.GetOrAdd(toolName, key => new ToolMetric { ToolName = key });
        lock (metric)
        {
            metric.TotalCalls++;
            if (denied)
            {
                metric.Denied++;
            }
            else if (success)
            {
                metric.Successful++;
            }
            else
            {
                metric.Failed++;
            }

            metric.TotalLatencyMs += latencyMs;
            metric.AvgLatencyMs = metric.TotalCalls > 0 ? (double)metric.TotalLatencyMs / metric.TotalCalls : 0;
        }
    }

    public void RecordWorkflow(string workflowName, long durationMs, bool success, string? failure = null)
    {
        var metric = _workflows.GetOrAdd(workflowName, key => new WorkflowMetric { WorkflowName = key });
        lock (metric)
        {
            metric.TotalExecutions++;
            if (success)
            {
                metric.Successful++;
            }
            else if (failure?.Contains("cancel", StringComparison.OrdinalIgnoreCase) == true)
            {
                metric.Cancelled++;
            }
            else
            {
                metric.Failed++;
            }

            metric.TotalDurationMs += durationMs;
            metric.AvgDurationMs = metric.TotalExecutions > 0 ? (double)metric.TotalDurationMs / metric.TotalExecutions : 0;
        }
    }

    public void RecordModel(string model, string? provider, long latencyMs, decimal? cost, ModelUsage? usage, bool success)
    {
        var metric = _models.GetOrAdd(model, key => new ModelMetric { Model = key });
        lock (metric)
        {
            metric.TotalCalls++;
            metric.TotalLatencyMs += latencyMs;
            metric.AvgLatencyMs = metric.TotalCalls > 0 ? (double)metric.TotalLatencyMs / metric.TotalCalls : 0;
            if (cost is not null)
            {
                metric.TotalCost += cost.Value;
            }

            if (usage is not null)
            {
                metric.TotalTokens += usage.TotalTokens;
            }
        }
    }

    public MetricsSnapshot Snapshot()
    {
        var agents = _agents.Values.OrderBy(a => a.AgentId).ToList();
        var tools = _tools.Values.OrderBy(t => t.ToolName).ToList();
        var workflows = _workflows.Values.OrderBy(w => w.WorkflowName).ToList();
        var models = _models.Values.OrderBy(m => m.Model).ToList();

        return new MetricsSnapshot
        {
            Agents = agents,
            Tools = tools,
            Workflows = workflows,
            Models = models,
            TotalFailures = agents.Sum(a => a.Failed) + tools.Sum(t => t.Failed) + workflows.Sum(w => w.Failed),
            TotalLatencyMs = agents.Sum(a => a.TotalLatencyMs) + tools.Sum(t => t.TotalLatencyMs) + models.Sum(m => m.TotalLatencyMs),
            TotalCost = agents.Sum(a => a.TotalCost) + models.Sum(m => m.TotalCost),
            TotalTokens = agents.Sum(a => a.TotalTokens) + models.Sum(m => m.TotalTokens),
            CapturedAt = DateTime.UtcNow
        };
    }

    public Task<MetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Snapshot());
    }
}
