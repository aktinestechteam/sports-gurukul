namespace SportsGurukul.Platform.AI.Models;

public enum StreamEventType
{
    Status,
    Message,
    Plan,
    ToolCall,
    ToolResult,
    Workflow,
    Approval,
    Metrics,
    Error,
    Done
}

public class AgentStreamEvent
{
    public Guid RunId { get; set; }
    public long Sequence { get; set; }
    public StreamEventType Type { get; set; }
    public string? Data { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static AgentStreamEvent Status(Guid runId, string status, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Status, Data = status, CorrelationId = correlationId };

    public static AgentStreamEvent Message(Guid runId, string message, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Message, Data = message, CorrelationId = correlationId };

    public static AgentStreamEvent Plan(Guid runId, string plan, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Plan, Data = plan, CorrelationId = correlationId };

    public static AgentStreamEvent ToolCall(Guid runId, string toolCall, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.ToolCall, Data = toolCall, CorrelationId = correlationId };

    public static AgentStreamEvent ToolResult(Guid runId, string result, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.ToolResult, Data = result, CorrelationId = correlationId };

    public static AgentStreamEvent Workflow(Guid runId, string status, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Workflow, Data = status, CorrelationId = correlationId };

    public static AgentStreamEvent Approval(Guid runId, string message, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Approval, Data = message, CorrelationId = correlationId };

    public static AgentStreamEvent Error(Guid runId, string error, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Error, Data = error, CorrelationId = correlationId };

    public static AgentStreamEvent Done(Guid runId, string? correlationId = null) =>
        new() { RunId = runId, Type = StreamEventType.Done, CorrelationId = correlationId };
}
