using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Events;
using SportsGurukul.Platform.AI.HumanInTheLoop;
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
using SportsGurukul.Platform.AI.Mcp;
using SportsGurukul.Platform.AI.Memory;
using SportsGurukul.Platform.AI.Model;
using SportsGurukul.Platform.AI.MultiAgent;
using SportsGurukul.Platform.AI.Observability;
using SportsGurukul.Platform.AI.Planning;
using SportsGurukul.Platform.AI.Runtime;
using SportsGurukul.Platform.AI.Security;
using SportsGurukul.Platform.AI.Services;
using SportsGurukul.Platform.AI.Streaming;
using SportsGurukul.Platform.AI.Tools;
using SportsGurukul.Platform.AI.Workflow;

namespace SportsGurukul.Platform.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAIPlatform(
        this IServiceCollection services,
        Action<AIPlatformOptions>? configureOptions = null)
    {
        var options = new AIPlatformOptions();
        configureOptions?.Invoke(options);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddSingleton(options);

        RegisterSecurity(services);
        RegisterModel(services);
        RegisterObservability(services);
        RegisterStreaming(services);
        RegisterMemory(services);
        RegisterHumanInTheLoop(services);
        RegisterTools(services);
        RegisterMcp(services);
        RegisterWorkflow(services);
        RegisterPlanning(services);
        RegisterRuntime(services);
        RegisterMultiAgent(services);
        RegisterServices(services);

        return services;
    }

    private static void RegisterSecurity(IServiceCollection services)
    {
        services.AddSingleton<ITenantContextAccessor, AsyncLocalTenantContextAccessor>();
        services.AddSingleton<ITenantIsolation, DefaultTenantIsolation>();
        services.AddSingleton<IPromptInjectionGuard, DefaultPromptInjectionGuard>();
        services.AddSingleton<IOutputValidator, DefaultOutputValidator>();
        services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();
    }

    private static void RegisterModel(IServiceCollection services)
    {
        services.AddSingleton<ILanguageModelFactory, InMemoryLanguageModelFactory>();
    }

    private static void RegisterObservability(IServiceCollection services)
    {
        services.AddSingleton<InMemoryMetricsCollector>();
        services.AddSingleton<IMetricsCollector>(sp => sp.GetRequiredService<InMemoryMetricsCollector>());
        services.AddSingleton<IMetricsReporter>(sp => sp.GetRequiredService<InMemoryMetricsCollector>());
        services.AddSingleton<IObservabilityService, ObservabilityService>();
    }

    private static void RegisterStreaming(IServiceCollection services)
    {
        services.AddSingleton<IAgentEventStream, InMemoryAgentEventStream>();
    }

    private static void RegisterMemory(IServiceCollection services)
    {
        services.AddSingleton<InMemoryMemoryStore>();
        services.AddSingleton<IWorkingMemoryStore>(sp => sp.GetRequiredService<InMemoryMemoryStore>());
        services.AddSingleton<ISessionMemoryStore>(sp => sp.GetRequiredService<InMemoryMemoryStore>());
        services.AddSingleton<ILongTermMemoryStore>(sp => sp.GetRequiredService<InMemoryMemoryStore>());
        services.AddSingleton<ISemanticMemoryStore>(sp => sp.GetRequiredService<InMemoryMemoryStore>());
        services.AddSingleton<IEpisodicMemoryStore>(sp => sp.GetRequiredService<InMemoryMemoryStore>());
        services.AddSingleton<IEmbeddingProvider, HashedEmbeddingProvider>();
        services.AddSingleton<IMemoryService, MemoryService>();
        services.AddSingleton<IAgentMemory>(sp => sp.GetRequiredService<IMemoryService>());
    }

    private static void RegisterHumanInTheLoop(IServiceCollection services)
    {
        services.AddSingleton<IApprovalStore, InMemoryApprovalStore>();
        services.AddSingleton<IApprovalCoordinator, ApprovalCoordinator>();
        services.AddSingleton<IApprovalService, ApprovalService>();
    }

    private static void RegisterTools(IServiceCollection services)
    {
        services.AddSingleton<IInternalApiGateway, StubInternalApiGateway>();
        services.AddSingleton<IRestApiClient, DefaultRestApiClient>();
        services.AddSingleton<IDatabaseQueryExecutor, StubDatabaseQueryExecutor>();
        services.AddSingleton<IKnowledgeSearcher, StubKnowledgeSearcher>();
        services.AddSingleton<INotificationGateway, StubNotificationGateway>();
        services.AddSingleton<IFinanceGateway, StubFinanceGateway>();
        services.AddSingleton<ISchedulingGateway, StubSchedulingGateway>();

        services.AddSingleton<ITool>(sp => new InternalApiTool(sp.GetRequiredService<IInternalApiGateway>()));
        services.AddSingleton<ITool>(sp => new DatabaseTool(sp.GetRequiredService<IDatabaseQueryExecutor>()));
        services.AddSingleton<ITool>(sp => new KnowledgeSearchTool(sp.GetRequiredService<IKnowledgeSearcher>()));
        services.AddSingleton<ITool>(sp => new NotificationTool(sp.GetRequiredService<INotificationGateway>()));
        services.AddSingleton<ITool>(sp => new FinanceTool(sp.GetRequiredService<IFinanceGateway>()));
        services.AddSingleton<ITool>(sp => new SchedulingTool(sp.GetRequiredService<ISchedulingGateway>()));

        services.AddSingleton<IToolRegistry, InMemoryToolRegistry>();
        services.AddSingleton<IToolAuthorization, DefaultToolAuthorization>();
        services.AddSingleton<IToolExecutor, DefaultToolExecutor>();
        services.AddSingleton<IToolService, ToolService>();
    }

    private static void RegisterMcp(IServiceCollection services)
    {
        services.AddSingleton<IMcpClientFactory, McpClientFactory>();
        services.AddSingleton<IMcpToolAdapter, DefaultMcpToolAdapter>();
        services.AddSingleton<IMcpServerRegistry, InMemoryMcpServerRegistry>();
    }

    private static void RegisterWorkflow(IServiceCollection services)
    {
        services.AddSingleton<IWorkflowStore, InMemoryWorkflowStore>();
        services.AddSingleton<IConditionEvaluator, SimpleConditionEvaluator>();
        services.AddSingleton<IWorkflowEngine, WorkflowEngine>();
        services.AddSingleton<IWorkflowService, WorkflowService>();
    }

    private static void RegisterPlanning(IServiceCollection services)
    {
        services.AddSingleton<IPlanningService, PlanningService>();
        services.AddSingleton<IReflectionService, ReflectionService>();
    }

    private static void RegisterRuntime(IServiceCollection services)
    {
        services.AddSingleton<IAgentRegistry, InMemoryAgentRegistry>();
        services.AddSingleton<IAgentLifecycleService, AgentLifecycleService>();
        services.AddSingleton<IAgentPlanner, DefaultAgentPlanner>();
        services.AddSingleton<IAgentExecutor, AgentExecutor>();
        services.AddSingleton<IAgentRuntime, AgentRuntime>();
    }

    private static void RegisterMultiAgent(IServiceCollection services)
    {
        services.AddSingleton<IAgentRouter, AgentRouter>();
        services.AddSingleton<IResultAggregator, ResultAggregator>();
        services.AddSingleton<ISupervisorAgent, SupervisorAgent>();
        services.AddSingleton<ICollaborationCoordinator, CollaborationCoordinator>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAgentService, AgentService>();
        services.AddSingleton<IDomainEventPublisher, MediatRDomainEventPublisher>();
    }
}
