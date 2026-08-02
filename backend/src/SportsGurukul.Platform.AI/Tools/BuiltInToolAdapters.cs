using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tools;

internal static class ToolArgumentExtensions
{
    public static object? GetValueOrDefault(this IDictionary<string, object?> arguments, string key) =>
        arguments.TryGetValue(key, out var value) ? value : null;
}

public abstract class GatewayTool : ITool
{
    protected GatewayTool(
        string name,
        string? description,
        ToolType type,
        bool requiresApproval = false,
        int? timeoutSeconds = null,
        string? permission = null,
        IReadOnlyList<string>? tags = null,
        IReadOnlyDictionary<string, string>? parameters = null)
    {
        Name = name;
        Description = description;
        Type = type;
        RequiresApproval = requiresApproval;
        TimeoutSeconds = timeoutSeconds;
        Permission = permission;
        Tags = tags ?? [];
        Parameters = parameters ?? new Dictionary<string, string>();
    }

    public string Name { get; }

    public string? Description { get; }

    public ToolType Type { get; }

    public bool RequiresApproval { get; }

    public int? TimeoutSeconds { get; }

    public string? Permission { get; }

    public IReadOnlyList<string> Tags { get; }

    public IReadOnlyDictionary<string, string> Parameters { get; }

    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken = default) =>
        ExecuteCoreAsync(call, cancellationToken);

    protected abstract Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken);
}

public class InternalApiTool : GatewayTool
{
    private readonly IInternalApiGateway _gateway;

    public InternalApiTool(IInternalApiGateway gateway, string name = "internal-api", string? operation = null, bool requiresApproval = false, int? timeoutSeconds = null)
        : base(name, "Invoke an internal API operation.", ToolType.InternalApi, requiresApproval, timeoutSeconds, "internal-api",
              ["internal", "api"], new Dictionary<string, string> { ["operation"] = "Internal operation name" })
    {
        _gateway = gateway;
        Operation = operation;
    }

    public string? Operation { get; }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var operation = Operation ?? call.Arguments.GetValueOrDefault("operation")?.ToString();
        if (string.IsNullOrWhiteSpace(operation))
        {
            return ToolResult.Fail("Operation name is required.");
        }

        var result = await _gateway.CallAsync(operation, call.Arguments, cancellationToken);
        return ToolResult.Ok(result);
    }
}

public class RestApiTool : GatewayTool
{
    private readonly IRestApiClient _client;
    private readonly Uri _endpoint;

    public RestApiTool(IRestApiClient client, string name, Uri endpoint, string? method = "GET", bool requiresApproval = false, int? timeoutSeconds = null)
        : base(name, $"Call REST endpoint {endpoint}", ToolType.RestApi, requiresApproval, timeoutSeconds, "rest-api",
              ["rest", "http"], new Dictionary<string, string> { ["method"] = "HTTP method", ["body"] = "Request body" })
    {
        _client = client;
        _endpoint = endpoint;
        Method = method ?? "GET";
    }

    public string? Method { get; }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var method = call.Arguments.GetValueOrDefault("method")?.ToString() ?? Method;
        var body = call.Arguments.TryGetValue("body", out var bodyValue) && bodyValue is IDictionary<string, object?> dict ? dict : null;

        try
        {
            var result = await _client.CallAsync(_endpoint, method ?? "GET", body, null, cancellationToken);
            return ToolResult.Ok(result);
        }
        catch (Exception ex)
        {
            return ToolResult.Fail(ex.Message);
        }
    }
}

public class DatabaseTool : GatewayTool
{
    private readonly IDatabaseQueryExecutor _executor;

    public DatabaseTool(IDatabaseQueryExecutor executor, string name = "database", bool requiresApproval = false, int? timeoutSeconds = null)
        : base(name, "Execute a read-only database query.", ToolType.Database, requiresApproval, timeoutSeconds, "database",
              ["database", "sql"], new Dictionary<string, string> { ["statement"] = "SQL statement", ["parameters"] = "Query parameters" })
    {
        _executor = executor;
    }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var statement = call.Arguments.GetValueOrDefault("statement")?.ToString();
        if (string.IsNullOrWhiteSpace(statement))
        {
            return ToolResult.Fail("Statement is required.");
        }

        var parameters = call.Arguments.TryGetValue("parameters", out var p) && p is IDictionary<string, object?> dict ? dict : null;
        var result = await _executor.ExecuteAsync(statement, parameters, cancellationToken);
        return ToolResult.Ok(result);
    }
}

public class KnowledgeSearchTool : GatewayTool
{
    private readonly IKnowledgeSearcher _searcher;

    public KnowledgeSearchTool(IKnowledgeSearcher searcher, string name = "knowledge-search", int? timeoutSeconds = null)
        : base(name, "Search the sports knowledge base.", ToolType.KnowledgeSearch, false, timeoutSeconds, "knowledge",
              ["knowledge", "search", "rag"], new Dictionary<string, string> { ["query"] = "Search query", ["topK"] = "Max results" })
    {
        _searcher = searcher;
    }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var query = call.Arguments.GetValueOrDefault("query")?.ToString();
        if (string.IsNullOrWhiteSpace(query))
        {
            return ToolResult.Fail("Query is required.");
        }

        var topK = int.TryParse(call.Arguments.GetValueOrDefault("topK")?.ToString(), out var parsed) ? parsed : 5;
        var results = await _searcher.SearchAsync(query, topK, cancellationToken);
        return ToolResult.Ok(results);
    }
}

public class NotificationTool : GatewayTool
{
    private readonly INotificationGateway _gateway;

    public NotificationTool(INotificationGateway gateway, string name = "notification", bool requiresApproval = false, int? timeoutSeconds = null)
        : base(name, "Send a notification through the notification platform.", ToolType.Notification, requiresApproval, timeoutSeconds, "notification",
              ["notification", "email", "sms", "push"], new Dictionary<string, string> { ["channel"] = "Notification channel", ["payload"] = "Notification payload" })
    {
        _gateway = gateway;
    }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var channel = call.Arguments.GetValueOrDefault("channel")?.ToString();
        if (string.IsNullOrWhiteSpace(channel))
        {
            return ToolResult.Fail("Channel is required.");
        }

        var payload = call.Arguments.TryGetValue("payload", out var p) && p is IDictionary<string, object?> dict ? dict : new Dictionary<string, object?>();
        var result = await _gateway.SendAsync(channel, payload, cancellationToken);
        return ToolResult.Ok(result);
    }
}

public class FinanceTool : GatewayTool
{
    private readonly IFinanceGateway _gateway;

    public FinanceTool(IFinanceGateway gateway, string name = "finance", bool requiresApproval = true, int? timeoutSeconds = null)
        : base(name, "Execute an operation on the finance platform.", ToolType.Finance, requiresApproval, timeoutSeconds, "finance",
              ["finance", "payment", "invoice", "ledger"], new Dictionary<string, string> { ["operation"] = "Finance operation", ["payload"] = "Operation payload" })
    {
        _gateway = gateway;
    }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var operation = call.Arguments.GetValueOrDefault("operation")?.ToString();
        if (string.IsNullOrWhiteSpace(operation))
        {
            return ToolResult.Fail("Operation is required.");
        }

        var payload = call.Arguments.TryGetValue("payload", out var p) && p is IDictionary<string, object?> dict ? dict : call.Arguments;
        var result = await _gateway.ExecuteAsync(operation, payload, cancellationToken);
        return ToolResult.Ok(result);
    }
}

public class SchedulingTool : GatewayTool
{
    private readonly ISchedulingGateway _gateway;

    public SchedulingTool(ISchedulingGateway gateway, string name = "scheduling", bool requiresApproval = false, int? timeoutSeconds = null)
        : base(name, "Execute an operation on the scheduling platform.", ToolType.Scheduling, requiresApproval, timeoutSeconds, "scheduling",
              ["scheduling", "booking", "calendar"], new Dictionary<string, string> { ["operation"] = "Scheduling operation", ["payload"] = "Operation payload" })
    {
        _gateway = gateway;
    }

    protected override async Task<ToolResult> ExecuteCoreAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var operation = call.Arguments.GetValueOrDefault("operation")?.ToString();
        if (string.IsNullOrWhiteSpace(operation))
        {
            return ToolResult.Fail("Operation is required.");
        }

        var payload = call.Arguments.TryGetValue("payload", out var p) && p is IDictionary<string, object?> dict ? dict : call.Arguments;
        var result = await _gateway.ExecuteAsync(operation, payload, cancellationToken);
        return ToolResult.Ok(result);
    }
}
