namespace SportsGurukul.Platform.AI.Models;

public class TokenUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens => PromptTokens + CompletionTokens;
    public string? Model { get; set; }
    public decimal? Cost { get; set; }
}

public class AgentMetric
{
    public string AgentId { get; set; } = string.Empty;
    public long TotalCalls { get; set; }
    public long Successful { get; set; }
    public long Failed { get; set; }
    public double AvgLatencyMs { get; set; }
    public long MaxLatencyMs { get; set; }
    public long TotalLatencyMs { get; set; }
    public decimal TotalCost { get; set; }
    public int TotalTokens { get; set; }
}

public class ToolMetric
{
    public string ToolName { get; set; } = string.Empty;
    public long TotalCalls { get; set; }
    public long Successful { get; set; }
    public long Failed { get; set; }
    public long Denied { get; set; }
    public double AvgLatencyMs { get; set; }
    public long TotalLatencyMs { get; set; }
}

public class WorkflowMetric
{
    public string WorkflowName { get; set; } = string.Empty;
    public long TotalExecutions { get; set; }
    public long Successful { get; set; }
    public long Failed { get; set; }
    public long Cancelled { get; set; }
    public double AvgDurationMs { get; set; }
    public long TotalDurationMs { get; set; }
}

public class ModelMetric
{
    public string Model { get; set; } = string.Empty;
    public long TotalCalls { get; set; }
    public int TotalTokens { get; set; }
    public decimal TotalCost { get; set; }
    public long TotalLatencyMs { get; set; }
    public double AvgLatencyMs { get; set; }
}

public class MetricsSnapshot
{
    public IReadOnlyList<AgentMetric> Agents { get; set; } = [];
    public IReadOnlyList<ToolMetric> Tools { get; set; } = [];
    public IReadOnlyList<WorkflowMetric> Workflows { get; set; } = [];
    public IReadOnlyList<ModelMetric> Models { get; set; } = [];
    public long TotalFailures { get; set; }
    public long TotalLatencyMs { get; set; }
    public decimal TotalCost { get; set; }
    public int TotalTokens { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}
