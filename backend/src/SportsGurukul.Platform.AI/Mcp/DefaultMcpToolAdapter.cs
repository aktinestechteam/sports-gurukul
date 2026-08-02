using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Mcp;

public class DefaultMcpToolAdapter : IMcpToolAdapter
{
    public ITool Adapt(IMcpClient client, McpToolInfo tool, McpAdapterOptions? options = null)
    {
        options ??= new McpAdapterOptions();
        return new AdaptedMcpTool(client, tool, options);
    }
}

public class AdaptedMcpTool : ITool
{
    private readonly IMcpClient _client;
    private readonly McpToolInfo _tool;
    private readonly McpAdapterOptions _options;

    public AdaptedMcpTool(IMcpClient client, McpToolInfo tool, McpAdapterOptions options)
    {
        _client = client;
        _tool = tool;
        _options = options;
    }

    public string Name => $"{_tool.ServerName}.{_tool.Name}";

    public string? Description => _tool.Description;

    public ToolType Type => ToolType.Mcp;

    public bool RequiresApproval => _options.RequiresApproval;

    public int? TimeoutSeconds => _options.TimeoutSeconds;

    public string? Permission => _options.Permission;

    public IReadOnlyList<string> Tags => _options.Tags;

    public IReadOnlyDictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken = default)
    {
        var response = await _client.InvokeToolAsync(_tool.Name, call.Arguments, cancellationToken);
        return response.Success
            ? ToolResult.Ok(response.Data)
            : ToolResult.Fail(response.Error ?? $"MCP tool '{_tool.Name}' failed.");
    }
}
