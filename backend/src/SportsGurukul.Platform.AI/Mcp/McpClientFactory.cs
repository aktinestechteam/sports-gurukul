using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Mcp;

public class McpClientFactory : IMcpClientFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public McpClientFactory(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory ?? Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
    }

    public IMcpServer CreateServer(McpServerInfo info) => info.Transport switch
    {
        McpTransportType.InMemory => new InMemoryMcpServer(info, _loggerFactory.CreateLogger<InMemoryMcpServer>()),
        _ => throw new AgentPlatformException(
            $"MCP transport '{info.Transport}' is not implemented yet. Provide an IMcpServer implementation via the registry.",
            "MCP_TRANSPORT_UNSUPPORTED")
    };

    public IMcpClient CreateClient(IMcpServer server)
    {
        if (server is InMemoryMcpServer inMemory)
        {
            return new InMemoryMcpClient(inMemory);
        }

        throw new AgentPlatformException(
            $"No client factory registered for MCP server '{server.Name}'. Register a custom IMcpClient.",
            "MCP_CLIENT_UNSUPPORTED");
    }
}
