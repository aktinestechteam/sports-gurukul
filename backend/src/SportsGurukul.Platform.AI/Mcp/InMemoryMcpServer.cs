using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Mcp;

public class InMemoryMcpServer : IMcpServer
{
    private readonly ConcurrentDictionary<string, McpToolHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IAsyncEnumerable<McpMessage>> _streams = new();
    private readonly ILogger<InMemoryMcpServer> _logger;

    public InMemoryMcpServer(McpServerInfo info, ILogger<InMemoryMcpServer>? logger = null)
    {
        Info = info;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryMcpServer>.Instance;
    }

    public delegate Task<McpInvokeResponse> McpToolHandler(McpInvokeRequest request, CancellationToken cancellationToken);

    public string Name => Info.Name;

    public McpServerInfo Info { get; }

    public InMemoryMcpServer AddTool(string name, string? description, McpToolHandler handler, string? inputSchema = null, string? outputSchema = null)
    {
        _handlers[name] = handler;
        _logger.LogInformation("MCP server '{Server}' registered tool '{Tool}'", Name, name);
        _handlersMeta[name] = new McpToolInfo
        {
            Name = name,
            Description = description,
            ServerName = Name,
            InputSchema = inputSchema,
            OutputSchema = outputSchema
        };
        return this;
    }

    public InMemoryMcpServer AddStreamingTool(string name, IAsyncEnumerable<McpMessage> stream)
    {
        _streams[name] = stream;
        return this;
    }

    private readonly ConcurrentDictionary<string, McpToolInfo> _handlersMeta = new();

    public Task<IMcpClient> ConnectAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IMcpClient>(new InMemoryMcpClient(this));

    public async Task<IReadOnlyList<McpToolInfo>> DiscoverToolsAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return _handlersMeta.Values.ToList();
    }

    internal Task<McpInvokeResponse> InvokeAsync(McpInvokeRequest request, CancellationToken cancellationToken)
    {
        if (_handlers.TryGetValue(request.ToolName, out var handler))
        {
            return handler(request, cancellationToken);
        }

        return Task.FromResult(new McpInvokeResponse
        {
            Success = false,
            Error = $"Tool '{request.ToolName}' not found on server '{Name}'."
        });
    }

    internal IAsyncEnumerable<McpMessage>? GetStream(string toolName) =>
        _streams.TryGetValue(toolName, out var stream) ? stream : null;
}

public class InMemoryMcpClient : IMcpClient
{
    private readonly InMemoryMcpServer _server;

    public InMemoryMcpClient(InMemoryMcpServer server)
    {
        _server = server;
        Name = server.Name;
    }

    public string Name { get; }

    public bool IsConnected { get; private set; } = true;

    public Task<McpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_server.Info.Capabilities);

    public Task<IReadOnlyList<McpToolInfo>> ListToolsAsync(CancellationToken cancellationToken = default) =>
        _server.DiscoverToolsAsync(cancellationToken);

    public Task<McpInvokeResponse> InvokeToolAsync(string toolName, IDictionary<string, object?> arguments, CancellationToken cancellationToken = default) =>
        _server.InvokeAsync(new McpInvokeRequest { ToolName = toolName, Arguments = arguments }, cancellationToken);

    public async IAsyncEnumerable<McpMessage> StreamAsync(string toolName, IDictionary<string, object?> arguments, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var stream = _server.GetStream(toolName);
        if (stream is null)
        {
            yield break;
        }

        await foreach (var message in stream.WithCancellation(cancellationToken))
        {
            yield return message;
        }
    }

    public Task<McpMessage> SendAsync(McpMessage message, CancellationToken cancellationToken = default) =>
        Task.FromResult(new McpMessage
        {
            Type = "ack",
            Content = $"Ack for {message.Id}",
            Metadata = new Dictionary<string, object?> { ["server"] = Name }
        });

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        IsConnected = false;
        return ValueTask.CompletedTask;
    }
}
