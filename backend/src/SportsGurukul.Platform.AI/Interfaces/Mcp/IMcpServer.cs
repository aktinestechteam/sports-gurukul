using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Mcp;

public interface IMcpServer
{
    string Name { get; }

    McpServerInfo Info { get; }

    Task<IMcpClient> ConnectAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<McpToolInfo>> DiscoverToolsAsync(CancellationToken cancellationToken = default);
}
