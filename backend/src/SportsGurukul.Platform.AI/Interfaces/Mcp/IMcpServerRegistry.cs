using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Mcp;

public interface IMcpServerRegistry
{
    Task<IMcpServer> RegisterAsync(IMcpServer server, CancellationToken cancellationToken = default);

    Task<bool> UnregisterAsync(string serverName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<McpServerInfo>> ListServersAsync(CancellationToken cancellationToken = default);

    Task<IMcpClient?> ConnectAsync(string serverName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<McpToolInfo>> DiscoverAllToolsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ITool>> AdaptToolsAsync(McpAdapterOptions? options = null, CancellationToken cancellationToken = default);
}
