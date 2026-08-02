using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Mcp;

public interface IMcpClientFactory
{
    IMcpServer CreateServer(McpServerInfo info);

    IMcpClient CreateClient(IMcpServer server);
}
