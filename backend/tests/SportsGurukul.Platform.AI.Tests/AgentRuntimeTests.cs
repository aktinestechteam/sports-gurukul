using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Services;

namespace SportsGurukul.Platform.AI.Tests;

public class AgentRuntimeTests
{
    private static ServiceProvider CreateProvider(Action<AIPlatformOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddAIPlatform(configure ?? (_ => { }));
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task RunAsync_CompletesToolDrivenAgent()
    {
        using var provider = CreateProvider(o => o.EnableSelfEvaluation = false);
        var agentService = provider.GetRequiredService<IAgentService>();

        await agentService.RegisterAsync(new AgentDefinition
        {
            Name = "coach",
            Description = "Sports coaching assistant",
            SystemPrompt = "You are a cricket coaching assistant.",
            MaxIterations = 5
        });

        var result = await agentService.RunAsync(new AgentRunRequest
        {
            AgentId = "coach",
            Goal = "Search knowledge base for player statistics; Schedule training",
            SessionId = "session-1",
            TenantId = "t1",
            UserId = "user-1"
        });

        Assert.Equal(AgentState.Completed, result.Status);
        Assert.False(string.IsNullOrEmpty(result.Answer));
        Assert.True(result.IterationCount > 0);
        Assert.NotEmpty(result.Tasks);
    }

    [Fact]
    public async Task RunAsync_UnknownAgentThrows()
    {
        using var provider = CreateProvider();
        var agentService = provider.GetRequiredService<IAgentService>();

        await Assert.ThrowsAsync<AgentNotFoundException>(() =>
            agentService.RunAsync(new AgentRunRequest { AgentId = "missing", Goal = "Do a thing" }));
    }

    [Fact]
    public async Task RunAsync_ApprovalFlowCompletesWhenApproved()
    {
        using var provider = CreateProvider(o => o.EnableSelfEvaluation = false);
        var agentService = provider.GetRequiredService<IAgentService>();
        var approvals = provider.GetRequiredService<Interfaces.HumanInTheLoop.IApprovalService>();
        var registry = provider.GetRequiredService<Interfaces.Tools.IToolRegistry>();

        await registry.RegisterAsync(new StubTestTool("payout", ToolType.Finance, requiresApproval: true));
        await agentService.RegisterAsync(new AgentDefinition
        {
            Name = "finance-agent",
            Description = "Finance operations agent",
            MaxIterations = 5
        });

        var runTask = agentService.RunAsync(new AgentRunRequest
        {
            AgentId = "finance-agent",
            Goal = "payout: process the payroll",
            TenantId = "t1"
        });

        var approvedAny = await ApprovePendingUntilDoneAsync(approvals, runTask);
        Assert.True(approvedAny, "Expected at least one approval request.");

        var result = await runTask;

        Assert.Equal(AgentState.Completed, result.Status);
    }

    [Fact]
    public async Task RunAsync_TracksRunResult()
    {
        using var provider = CreateProvider(o => o.EnableSelfEvaluation = false);
        var agentService = provider.GetRequiredService<IAgentService>();

        await agentService.RegisterAsync(new AgentDefinition { Name = "worker", MaxIterations = 5 });

        var result = await agentService.RunAsync(new AgentRunRequest { AgentId = "worker", Goal = "Do simple work" });

        var fetched = await agentService.GetRunAsync(result.RunId);
        Assert.NotNull(fetched);
        Assert.Equal(result.RunId, fetched!.RunId);
        Assert.Equal(AgentState.Completed, result.Status);
        Assert.False(await agentService.CancelAsync(Guid.NewGuid()));
    }

    private static async Task<bool> ApprovePendingUntilDoneAsync(Interfaces.HumanInTheLoop.IApprovalService approvals, Task<AgentRunResult> runTask)
    {
        var approvedAny = false;
        var deadline = DateTime.UtcNow.AddSeconds(15);

        while (DateTime.UtcNow < deadline && !runTask.IsCompleted)
        {
            var pending = await approvals.GetPendingAsync();
            foreach (var request in pending)
            {
                try
                {
                    await approvals.ApproveAsync(request.Id, "controller-1", "Approved");
                    approvedAny = true;
                }
                catch
                {
                }
            }

            await Task.Delay(50);
        }

        return approvedAny;
    }
}
