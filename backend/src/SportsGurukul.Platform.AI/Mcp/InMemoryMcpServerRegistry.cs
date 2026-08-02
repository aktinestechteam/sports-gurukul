using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Mcp;

public class InMemoryMcpServerRegistry : IMcpServerRegistry
{
    private readonly ConcurrentDictionary<string, IMcpServer> _servers = new(StringComparer.OrdinalIgnoreCase);
    private readonly IMcpClientFactory _clientFactory;
    private readonly IMcpToolAdapter _toolAdapter;
    private readonly ILogger<InMemoryMcpServerRegistry> _logger;

    public InMemoryMcpServerRegistry(
        IMcpClientFactory clientFactory,
        IMcpToolAdapter toolAdapter,
        ILogger<InMemoryMcpServerRegistry>? logger = null)
    {
        _clientFactory = clientFactory;
        _toolAdapter = toolAdapter;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryMcpServerRegistry>.Instance;
    }

    public Task<IMcpServer> RegisterAsync(IMcpServer server, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(server);
        _servers[server.Name] = server;
        _logger.LogInformation("Registered MCP server '{Server}' (transport {Transport})", server.Name, server.Info.Transport);
        return Task.FromResult(server);
    }

    public Task<bool> UnregisterAsync(string serverName, CancellationToken cancellationToken = default)
    {
        var removed = _servers.TryRemove(serverName, out _);
        return Task.FromResult(removed);
    }

    public Task<IReadOnlyList<McpServerInfo>> ListServersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<McpServerInfo>>(_servers.Values.Select(s => s.Info).ToList());

    public Task<IMcpClient?> ConnectAsync(string serverName, CancellationToken cancellationToken = default)
    {
        if (!_servers.TryGetValue(serverName, out var server))
        {
            return Task.FromResult<IMcpClient?>(null);
        }

        return ConnectCoreAsync(server, cancellationToken);
    }

    private async Task<IMcpClient?> ConnectCoreAsync(IMcpServer server, CancellationToken cancellationToken)
    {
        return await server.ConnectAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<McpToolInfo>> DiscoverAllToolsAsync(CancellationToken cancellationToken = default)
    {
        var tools = new List<McpToolInfo>();
        foreach (var server in _servers.Values)
        {
            tools.AddRange(await server.DiscoverToolsAsync(cancellationToken));
        }

        return tools;
    }

    public async Task<IReadOnlyList<ITool>> AdaptToolsAsync(McpAdapterOptions? options = null, CancellationToken cancellationToken = default)
    {
        var adapted = new List<ITool>();
        foreach (var server in _servers.Values)
        {
            var client = await server.ConnectAsync(cancellationToken);
            var tools = await client.ListToolsAsync(cancellationToken);
            foreach (var tool in tools)
            {
                adapted.Add(_toolAdapter.Adapt(client, tool, options));
            }
        }

        return adapted;
    }
}
