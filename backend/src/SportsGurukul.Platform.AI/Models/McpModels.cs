namespace SportsGurukul.Platform.AI.Models;

public enum McpTransportType
{
    Stdio,
    Http,
    Sse,
    WebSocket,
    InMemory,
    Custom
}

public class McpServerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public McpTransportType Transport { get; set; } = McpTransportType.Http;
    public Uri? Endpoint { get; set; }
    public string? Description { get; set; }
    public McpCapabilities Capabilities { get; set; } = new();
}

public class McpCapabilities
{
    public bool Tools { get; set; } = true;
    public bool Resources { get; set; }
    public bool Prompts { get; set; }
    public bool Streaming { get; set; }
    public bool Notifications { get; set; }
}

public class McpToolInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public string? InputSchema { get; set; }
    public string? OutputSchema { get; set; }
}

public class McpMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string? Type { get; set; }
    public string? Content { get; set; }
    public IDictionary<string, object?>? Metadata { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class McpInvokeRequest
{
    public string ToolName { get; set; } = string.Empty;
    public IDictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();
}

public class McpInvokeResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }
    public IReadOnlyList<McpMessage>? Stream { get; set; }
}

public class McpAdapterOptions
{
    public bool RequiresApproval { get; set; }
    public int? TimeoutSeconds { get; set; }
    public string? Permission { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = [];
}
