using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Mcp;

public interface IMcpClient : IAsyncDisposable
{
    string Name { get; }

    bool IsConnected { get; }

    Task<McpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<McpToolInfo>> ListToolsAsync(CancellationToken cancellationToken = default);

    Task<McpInvokeResponse> InvokeToolAsync(string toolName, IDictionary<string, object?> arguments, CancellationToken cancellationToken = default);

    IAsyncEnumerable<McpMessage> StreamAsync(string toolName, IDictionary<string, object?> arguments, CancellationToken cancellationToken = default);

    Task<McpMessage> SendAsync(McpMessage message, CancellationToken cancellationToken = default);

    Task DisconnectAsync(CancellationToken cancellationToken = default);
}
