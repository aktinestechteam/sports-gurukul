using SportsGurukul.Platform.AI.Interfaces.Tools;

namespace SportsGurukul.Platform.AI.Models;

public enum ToolType
{
    InternalApi,
    RestApi,
    Database,
    KnowledgeSearch,
    Notification,
    Finance,
    Scheduling,
    Mcp,
    Custom
}

public enum ToolCallStatus
{
    Pending,
    Running,
    Authorized,
    Denied,
    AwaitingApproval,
    Succeeded,
    Failed,
    TimedOut
}

public class ToolDescriptor
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ToolType Type { get; set; }
    public IReadOnlyDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    public bool RequiresApproval { get; set; }
    public int? TimeoutSeconds { get; set; }
    public string? Permission { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = [];

    public static ToolDescriptor From(ITool tool) => new()
    {
        Name = tool.Name,
        Description = tool.Description,
        Type = tool.Type,
        Parameters = tool.Parameters,
        RequiresApproval = tool.RequiresApproval,
        TimeoutSeconds = tool.TimeoutSeconds,
        Permission = tool.Permission,
        Tags = tool.Tags
    };
}

public class ToolExecutionContext
{
    public string? AgentId { get; set; }
    public string? RunId { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public string? UserId { get; set; }
    public string? CorrelationId { get; set; }
}

public class ToolCall
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ToolName { get; set; } = string.Empty;
    public IDictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();
    public ToolCallStatus Status { get; set; } = ToolCallStatus.Pending;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public int RetryCount { get; set; }
    public Guid? ApprovalRequestId { get; set; }
}

public class ToolResult
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }
    public long? DurationMs { get; set; }
    public ModelUsage? Usage { get; set; }
    public IReadOnlyList<string>? Warnings { get; set; }

    public static ToolResult Ok(object? data, long? durationMs = null) =>
        new() { Success = true, Data = data, DurationMs = durationMs };

    public static ToolResult Fail(string error, long? durationMs = null) =>
        new() { Success = false, Error = error, DurationMs = durationMs };
}

public class ToolAuthorizationDecision
{
    public bool Allowed { get; set; }
    public string? Reason { get; set; }
    public bool RequiresApproval { get; set; }
    public string? Policy { get; set; }

    public static ToolAuthorizationDecision Allow(string? reason = null, bool requiresApproval = false) =>
        new() { Allowed = true, Reason = reason, RequiresApproval = requiresApproval };

    public static ToolAuthorizationDecision Deny(string reason) =>
        new() { Allowed = false, Reason = reason };
}
