using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Mcp;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tests;

public class McpTests
{
    [Fact]
    public async Task Server_ConnectAndDiscoverTools()
    {
        var server = new InMemoryMcpServer(new McpServerInfo
        {
            Name = "cricket-stats",
            Transport = McpTransportType.InMemory
        }, NullLogger<InMemoryMcpServer>.Instance);

        server.AddTool("batting-averages", "Get player batting averages", (request, _) =>
            Task.FromResult(new McpInvokeResponse { Success = true, Data = new { average = 42.5 } }));

        var client = await server.ConnectAsync();
        var tools = await client.ListToolsAsync();

        Assert.True(client.IsConnected);
        Assert.Single(tools);
        Assert.Equal("batting-averages", tools[0].Name);
    }

    [Fact]
    public async Task Client_InvokeToolReturnsResult()
    {
        var server = new InMemoryMcpServer(new McpServerInfo { Name = "srv" });
        server.AddTool("echo", "Echo tool", (request, _) =>
            Task.FromResult(new McpInvokeResponse
            {
                Success = true,
                Data = request.Arguments.TryGetValue("message", out var m) ? m?.ToString() : null
            }));

        var client = await server.ConnectAsync();
        var response = await client.InvokeToolAsync("echo", new Dictionary<string, object?> { ["message"] = "hi" });

        Assert.True(response.Success);
        Assert.Equal("hi", response.Data);
    }

    [Fact]
    public async Task Client_UnknownToolReturnsFailure()
    {
        var server = new InMemoryMcpServer(new McpServerInfo { Name = "srv" });
        var client = await server.ConnectAsync();

        var response = await client.InvokeToolAsync("missing", new Dictionary<string, object?>());

        Assert.False(response.Success);
        Assert.Contains("not found", response.Error);
    }

    [Fact]
    public async Task Client_StreamsToolMessages()
    {
        var server = new InMemoryMcpServer(new McpServerInfo { Name = "srv" });
        server.AddStreamingTool("live-score", StreamMessages());

        var client = await server.ConnectAsync();
        var received = new List<string>();
        await foreach (var message in client.StreamAsync("live-score", new Dictionary<string, object?>()))
        {
            received.Add(message.Content!);
        }

        Assert.Equal(["first", "second"], received);
    }

    [Fact]
    public async Task Adapter_AdaptsToolToItool()
    {
        var server = new InMemoryMcpServer(new McpServerInfo { Name = "srv" });
        server.AddTool("weather", "Get weather", (request, _) =>
            Task.FromResult(new McpInvokeResponse { Success = true, Data = "sunny" }));

        var client = await server.ConnectAsync();
        var tools = await client.ListToolsAsync();
        var adapter = new DefaultMcpToolAdapter();
        var tool = adapter.Adapt(client, tools[0], new McpAdapterOptions { RequiresApproval = true });

        Assert.Equal("srv.weather", tool.Name);
        Assert.Equal(ToolType.Mcp, tool.Type);
        Assert.True(tool.RequiresApproval);

        var result = await tool.ExecuteAsync(new ToolCall { ToolName = "weather" });
        Assert.True(result.Success);
        Assert.Equal("sunny", result.Data);
    }

    [Fact]
    public void McpClientFactory_ThrowsForUnsupportedTransport()
    {
        var factory = new McpClientFactory();

        Assert.Throws<AgentPlatformException>(() =>
            factory.CreateServer(new McpServerInfo { Name = "x", Transport = McpTransportType.Http }));
    }

    [Fact]
    public async Task McpServerRegistry_DiscoversAndAdaptsTools()
    {
        var clientFactory = new McpClientFactory(NullLoggerFactory.Instance);
        var adapter = new DefaultMcpToolAdapter();
        var registry = new InMemoryMcpServerRegistry(clientFactory, adapter);

        var server = new InMemoryMcpServer(new McpServerInfo { Name = "stats", Transport = McpTransportType.InMemory });
        server.AddTool("totals", "Get totals", (request, _) =>
            Task.FromResult(new McpInvokeResponse { Success = true, Data = 100 }));

        await registry.RegisterAsync(server);

        var servers = await registry.ListServersAsync();
        Assert.Single(servers);

        var adapted = await registry.AdaptToolsAsync();
        Assert.Single(adapted);
        Assert.Equal("stats.totals", adapted[0].Name);
    }

    private static async IAsyncEnumerable<McpMessage> StreamMessages()
    {
        yield return new McpMessage { Content = "first" };
        await Task.Yield();
        yield return new McpMessage { Content = "second" };
    }
}
