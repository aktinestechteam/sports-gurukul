using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Mcp;

public interface IMcpToolAdapter
{
    ITool Adapt(IMcpClient client, McpToolInfo tool, McpAdapterOptions? options = null);
}
