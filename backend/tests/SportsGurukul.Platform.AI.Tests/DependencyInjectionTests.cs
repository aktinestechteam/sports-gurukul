using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.Mcp;
using SportsGurukul.Platform.AI.Interfaces.Memory;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Interfaces.Planning;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Interfaces.Workflow;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Services;

namespace SportsGurukul.Platform.AI.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddAIPlatform_ResolvesCoreServices()
    {
        var services = new ServiceCollection();
        services.AddAIPlatform(o => o.MaxAgentIterations = 3);

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<AIPlatformOptions>());
        Assert.NotNull(provider.GetRequiredService<IAgentRegistry>());
        Assert.NotNull(provider.GetRequiredService<IAgentLifecycleService>());
        Assert.NotNull(provider.GetRequiredService<IAgentPlanner>());
        Assert.NotNull(provider.GetRequiredService<IAgentExecutor>());
        Assert.NotNull(provider.GetRequiredService<IAgentRuntime>());
        Assert.NotNull(provider.GetRequiredService<IAgentService>());
        Assert.NotNull(provider.GetRequiredService<IAgentMemory>());
        Assert.NotNull(provider.GetRequiredService<IMemoryService>());
        Assert.NotNull(provider.GetRequiredService<IPlanningService>());
        Assert.NotNull(provider.GetRequiredService<IReflectionService>());
        Assert.NotNull(provider.GetRequiredService<IToolRegistry>());
        Assert.NotNull(provider.GetRequiredService<IToolExecutor>());
        Assert.NotNull(provider.GetRequiredService<IToolService>());
        Assert.NotNull(provider.GetRequiredService<IWorkflowStore>());
        Assert.NotNull(provider.GetRequiredService<IWorkflowEngine>());
        Assert.NotNull(provider.GetRequiredService<IWorkflowService>());
        Assert.NotNull(provider.GetRequiredService<IApprovalService>());
        Assert.NotNull(provider.GetRequiredService<IApprovalStore>());
        Assert.NotNull(provider.GetRequiredService<IMcpClientFactory>());
        Assert.NotNull(provider.GetRequiredService<IMcpServerRegistry>());
        Assert.NotNull(provider.GetRequiredService<IAgentEventStream>());
        Assert.NotNull(provider.GetRequiredService<IMetricsCollector>());
        Assert.NotNull(provider.GetRequiredService<IObservabilityService>());
        Assert.NotNull(provider.GetRequiredService<ITenantContextAccessor>());
        Assert.NotNull(provider.GetRequiredService<ITenantIsolation>());
        Assert.NotNull(provider.GetRequiredService<IPromptInjectionGuard>());
        Assert.NotNull(provider.GetRequiredService<IOutputValidator>());
        Assert.NotNull(provider.GetRequiredService<IAuditLogger>());
        Assert.NotNull(provider.GetRequiredService<ILanguageModelFactory>());
        Assert.NotNull(provider.GetRequiredService<ISupervisorAgent>());
        Assert.NotNull(provider.GetRequiredService<ICollaborationCoordinator>());
        Assert.NotNull(provider.GetRequiredService<IAgentRouter>());
        Assert.NotNull(provider.GetRequiredService<IResultAggregator>());
    }

    [Fact]
    public async Task AddAIPlatform_RegistersBuiltInTools()
    {
        var services = new ServiceCollection();
        services.AddAIPlatform();

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<IToolRegistry>();

        var tools = await registry.GetAllAsync();

        Assert.Contains(tools, t => t.Name == "internal-api");
        Assert.Contains(tools, t => t.Name == "database");
        Assert.Contains(tools, t => t.Name == "knowledge-search");
        Assert.Contains(tools, t => t.Name == "notification");
        Assert.Contains(tools, t => t.Name == "finance" && t.RequiresApproval);
        Assert.Contains(tools, t => t.Name == "scheduling");
    }

    [Fact]
    public async Task AddAIPlatform_LanguageModelFactory_ReturnsStubByDefault()
    {
        var services = new ServiceCollection();
        services.AddAIPlatform();

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ILanguageModelFactory>();

        var model = factory.Create("unknown-provider", "some-model");

        Assert.Equal("some-model", model.Model);
        var response = await model.GenerateAsync([ModelMessage.User("hello")]);
        Assert.False(string.IsNullOrWhiteSpace(response.Content));
    }
}
