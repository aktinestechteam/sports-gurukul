using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Observability;

public interface IMetricsCollector
{
    void RecordAgent(string agentId, long latencyMs, decimal cost, ModelUsage? usage, bool success, string? failure = null);

    void RecordTool(string toolName, string? agentId, long latencyMs, bool success, string? failure = null, bool denied = false);

    void RecordWorkflow(string workflowName, long durationMs, bool success, string? failure = null);

    void RecordModel(string model, string? provider, long latencyMs, decimal? cost, ModelUsage? usage, bool success);

    MetricsSnapshot Snapshot();
}
