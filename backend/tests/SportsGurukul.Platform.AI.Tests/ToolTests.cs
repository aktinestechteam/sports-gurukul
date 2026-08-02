using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Runtime;
using SportsGurukul.Platform.AI.Tools;

namespace SportsGurukul.Platform.AI.Tests;

public class ToolTests
{
    [Fact]
    public async Task Registry_RegisterAndGetTool()
    {
        var registry = new InMemoryToolRegistry();
        var tool = new StubTestTool("echo", ToolType.Custom);

        await registry.RegisterAsync(tool);

        var retrieved = await registry.GetAsync("echo");
        Assert.NotNull(retrieved);
        Assert.Equal("echo", retrieved!.Name);
        Assert.Contains(retrieved, await registry.GetAllAsync());
    }

    [Fact]
    public async Task Registry_SeedBuiltInTools()
    {
        var registry = new InMemoryToolRegistry(
            [new StubTestTool("a", ToolType.Custom), new StubTestTool("b", ToolType.Custom)]);

        var tools = await registry.GetAllAsync();

        Assert.Equal(2, tools.Count);
    }

    [Fact]
    public async Task Registry_UnregisterRemovesTool()
    {
        var registry = new InMemoryToolRegistry();
        await registry.RegisterAsync(new StubTestTool("x", ToolType.Custom));

        var removed = await registry.UnregisterAsync("x");

        Assert.True(removed);
        Assert.Null(await registry.GetAsync("x"));
    }

    [Fact]
    public async Task DefaultToolAuthorization_RequiresApprovalForApprovalTools()
    {
        var authorization = new DefaultToolAuthorization();
        var tool = new StubTestTool("pay", ToolType.Finance, requiresApproval: true);

        var decision = await authorization.AuthorizeAsync(tool, new ToolExecutionContext());

        Assert.True(decision.Allowed);
        Assert.True(decision.RequiresApproval);
    }

    [Fact]
    public void ToolDescriptor_FromTool()
    {
        var tool = new StubTestTool("search", ToolType.KnowledgeSearch, tags: ["rag"]);

        var descriptor = ToolDescriptor.From(tool);

        Assert.Equal("search", descriptor.Name);
        Assert.Equal(ToolType.KnowledgeSearch, descriptor.Type);
        Assert.Contains("rag", descriptor.Tags);
    }

    [Fact]
    public async Task DefaultToolExecutor_ExecutesToolAndRecordsMetrics()
    {
        var registry = new InMemoryToolRegistry();
        await registry.RegisterAsync(new StubTestTool("echo", ToolType.Custom));

        var metrics = new Observability.InMemoryMetricsCollector();
        var executor = new DefaultToolExecutor(
            registry,
            new DefaultToolAuthorization(),
            new HumanInTheLoop.ApprovalService(
                new HumanInTheLoop.InMemoryApprovalStore(),
                new HumanInTheLoop.ApprovalCoordinator(new HumanInTheLoop.InMemoryApprovalStore()),
                new AIPlatformOptions()),
            new Streaming.InMemoryAgentEventStream(),
            metrics,
            new Security.InMemoryAuditLogger(),
            new AIPlatformOptions());

        var context = new ToolExecutionContext { RunId = Guid.NewGuid().ToString(), TenantId = "t1" };
        var result = await executor.ExecuteAsync(
            "echo",
            new Dictionary<string, object?> { ["message"] = "hello" },
            context);

        Assert.True(result.Success);
        var snapshot = metrics.Snapshot();
        Assert.Single(snapshot.Tools);
        Assert.Equal(1, snapshot.Tools[0].TotalCalls);
    }
}

public class StubTestTool : ITool
{
    public StubTestTool(string name, ToolType type, bool requiresApproval = false, IReadOnlyList<string>? tags = null)
    {
        Name = name;
        Type = type;
        RequiresApproval = requiresApproval;
        Tags = tags ?? [];
    }

    public string Name { get; }

    public string? Description => "Test tool";

    public ToolType Type { get; }

    public bool RequiresApproval { get; }

    public int? TimeoutSeconds => null;

    public string? Permission => null;

    public IReadOnlyList<string> Tags { get; }

    public IReadOnlyDictionary<string, string> Parameters =>
        new Dictionary<string, string> { ["message"] = "Message to echo" };

    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken = default)
    {
        var message = call.Arguments.TryGetValue("message", out var m) ? m?.ToString() : "empty";
        return Task.FromResult(ToolResult.Ok(new Dictionary<string, object?> { ["echo"] = message }));
    }
}
