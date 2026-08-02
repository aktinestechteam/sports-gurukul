using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Observability;
using SportsGurukul.Platform.AI.Streaming;
using SportsGurukul.Platform.AI.Tools;
using SportsGurukul.Platform.AI.Workflow;

namespace SportsGurukul.Platform.AI.Tests;

public class WorkflowEngineTests
{
    private readonly AIPlatformOptions _options = new() { WorkflowRetryDelaySeconds = 0 };
    private readonly InMemoryWorkflowStore _store = new();
    private readonly InMemoryToolRegistry _registry = new();
    private readonly InMemoryMetricsCollector _metrics = new();
    private readonly IApprovalService _approval;
    private readonly WorkflowEngine _engine;

    public WorkflowEngineTests()
    {
        var approvalStore = new InMemoryApprovalStore();
        _approval = new ApprovalService(approvalStore, new ApprovalCoordinator(approvalStore), _options);

        var executor = new DefaultToolExecutor(
            _registry,
            new DefaultToolAuthorization(),
            _approval,
            new InMemoryAgentEventStream(),
            _metrics,
            new Security.InMemoryAuditLogger(),
            _options);

        _engine = new WorkflowEngine(
            _store,
            executor,
            _approval,
            new SimpleConditionEvaluator(),
            new InMemoryAgentEventStream(),
            _metrics,
            _options);
    }

    [Fact]
    public async Task StartAsync_CompletesSequentialToolSteps()
    {
        await _registry.RegisterAsync(new StubTestTool("echo", ToolType.Custom));

        var definition = new WorkflowDefinition
        {
            Name = "sequential",
            Version = 1,
            Steps =
            [
                new WorkflowStepDefinition { Id = "a", Name = "Echo A", ToolName = "echo", ToolArguments = new() { ["message"] = "A" } },
                new WorkflowStepDefinition { Id = "b", Name = "Echo B", ToolName = "echo", DependsOn = ["a"], ToolArguments = new() { ["message"] = "B" } }
            ]
        };

        var execution = await _engine.StartAsync(definition);

        Assert.Equal(WorkflowStatus.Completed, execution.Status);
        Assert.All(execution.Steps, s => Assert.Equal(WorkflowStepStatus.Succeeded, s.Status));
        Assert.True(execution.Revision > 0);
    }

    [Fact]
    public async Task StartAsync_StoresToolOutputInState()
    {
        await _registry.RegisterAsync(new StubTestTool("echo", ToolType.Custom));

        var definition = new WorkflowDefinition
        {
            Name = "stateful",
            Version = 1,
            Steps = [new WorkflowStepDefinition { Id = "a", Name = "Echo", ToolName = "echo", ToolArguments = new() { ["message"] = "world" } }]
        };

        var execution = await _engine.StartAsync(definition);

        Assert.Equal(WorkflowStatus.Completed, execution.Status);
        Assert.NotNull(execution.State["a"]);
    }

    [Fact]
    public async Task StartAsync_SkipsStepWhenConditionFalse()
    {
        var definition = new WorkflowDefinition
        {
            Name = "conditional",
            Version = 1,
            Steps =
            [
                new WorkflowStepDefinition
                {
                    Id = "c",
                    Name = "Conditional step",
                    ToolName = "echo",
                    Condition = "flag exists",
                    ToolArguments = new() { ["message"] = "x" }
                }
            ]
        };

        var execution = await _engine.StartAsync(definition);

        Assert.Equal(WorkflowStatus.Completed, execution.Status);
        Assert.Equal(WorkflowStepStatus.Skipped, execution.Steps[0].Status);
    }

    [Fact]
    public async Task ApprovalStep_WaitsAndCompletesOnApproval()
    {
        var definition = new WorkflowDefinition
        {
            Name = "approved",
            Version = 1,
            Steps =
            [
                new WorkflowStepDefinition { Id = "a", Name = "Manual approval", Type = WorkflowStepType.Approval },
                new WorkflowStepDefinition { Id = "b", Name = "After approval", ToolName = "echo", DependsOn = ["a"], ToolArguments = new() { ["message"] = "go" } }
            ]
        };

        await _registry.RegisterAsync(new StubTestTool("echo", ToolType.Custom));

        var execution = await _engine.StartAsync(definition);

        Assert.Equal(WorkflowStatus.WaitingForApproval, execution.Status);
        Assert.Equal(WorkflowStepStatus.WaitingForApproval, execution.Steps.First(s => s.StepId == "a").Status);

        var pending = await _approval.GetPendingAsync();
        var request = Assert.Single(pending);
        await _approval.ApproveAsync(request.Id, "manager");

        var resumed = await _engine.ResumeAsync(execution.Id);

        Assert.Equal(WorkflowStatus.Completed, resumed.Status);
        Assert.All(resumed.Steps, s => Assert.Equal(WorkflowStepStatus.Succeeded, s.Status));
    }

    [Fact]
    public async Task ApprovalStep_RejectionFailsWorkflow()
    {
        var definition = new WorkflowDefinition
        {
            Name = "rejected",
            Version = 1,
            Steps = [new WorkflowStepDefinition { Id = "a", Name = "Manual approval", Type = WorkflowStepType.Approval }]
        };

        var execution = await _engine.StartAsync(definition);
        var pending = await _approval.GetPendingAsync();
        var request = Assert.Single(pending);
        await _approval.RejectAsync(request.Id, "denied");

        var resumed = await _engine.ResumeAsync(execution.Id);

        Assert.Equal(WorkflowStatus.Failed, resumed.Status);
    }

    [Fact]
    public async Task CancelAsync_CancelsPendingWorkflow()
    {
        var definition = new WorkflowDefinition
        {
            Name = "cancellable",
            Version = 1,
            Steps = [new WorkflowStepDefinition { Id = "a", Name = "Manual approval", Type = WorkflowStepType.Approval }]
        };

        var execution = await _engine.StartAsync(definition);

        var cancelled = await _engine.CancelAsync(execution.Id, "stopped");

        Assert.Equal(WorkflowStatus.Cancelled, cancelled.Status);
        Assert.All(cancelled.Steps, s => Assert.Equal(WorkflowStepStatus.Cancelled, s.Status));
    }

    [Fact]
    public async Task GetAndListReturnExecutions()
    {
        var definition = new WorkflowDefinition
        {
            Name = "listing",
            Version = 1,
            Steps = [new WorkflowStepDefinition { Id = "a", Name = "noop", Type = WorkflowStepType.Condition }]
        };

        var execution = await _engine.StartAsync(definition);

        var fetched = await _engine.GetAsync(execution.Id);
        var all = await _engine.ListAsync();

        Assert.NotNull(fetched);
        Assert.Contains(all, e => e.Id == execution.Id);
    }
}
