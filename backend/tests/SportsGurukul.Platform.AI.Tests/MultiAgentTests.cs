using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Model;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.MultiAgent;

namespace SportsGurukul.Platform.AI.Tests;

public class MultiAgentTests
{
    private static WorkerAgent Worker(string name, params string[] capabilities) =>
        new(name, capabilities, new StubLanguageModel("stub", name), logger: NullLogger<WorkerAgent>.Instance);

    [Fact]
    public async Task Router_RoutesToCapableWorker()
    {
        var router = new AgentRouter();
        var workers = new List<IWorkerAgent>
        {
            Worker("finance", "finance", "payment"),
            Worker("training", "training", "schedule")
        };

        var decision = await router.RouteAsync(
            new DelegatedTask { Goal = "process the payment invoice" },
            workers);

        Assert.Equal("finance", decision.SelectedAgentId);
        Assert.True(decision.Confidence > 0);
    }

    [Fact]
    public async Task Router_ReturnsNullWhenNoCapabilityMatches()
    {
        var router = new AgentRouter();
        var workers = new List<IWorkerAgent> { Worker("finance", "finance") };

        var decision = await router.RouteAsync(
            new DelegatedTask { Goal = "cooking recipe for dinner" },
            workers);

        Assert.Null(decision.SelectedAgentId);
    }

    [Fact]
    public async Task Router_HonorsExplicitAssignment()
    {
        var router = new AgentRouter();
        var workers = new List<IWorkerAgent>
        {
            Worker("a", "alpha"),
            Worker("b", "beta")
        };

        var decision = await router.RouteAsync(
            new DelegatedTask { Goal = "alpha task", AssignedAgentId = "b" },
            workers);

        Assert.Equal("b", decision.SelectedAgentId);
    }

    [Fact]
    public async Task Aggregator_FirstSuccess_ReturnsFirstSuccessfulAnswer()
    {
        var aggregator = new ResultAggregator();
        var results = new List<DelegatedTaskResult>
        {
            new() { TaskId = Guid.NewGuid(), Succeeded = false, Error = "boom" },
            new() { TaskId = Guid.NewGuid(), Succeeded = true, Answer = "first good", AgentId = "a" },
            new() { TaskId = Guid.NewGuid(), Succeeded = true, Answer = "second good", AgentId = "b" }
        };

        var result = await aggregator.AggregateAsync(results, AggregationStrategy.FirstSuccess);

        Assert.True(result.Succeeded);
        Assert.Equal("first good", result.Answer);
    }

    [Fact]
    public async Task Aggregator_Vote_PicksMostFrequentAnswer()
    {
        var aggregator = new ResultAggregator();
        var results = new List<DelegatedTaskResult>
        {
            new() { TaskId = Guid.NewGuid(), Succeeded = true, Answer = "yes", AgentId = "a" },
            new() { TaskId = Guid.NewGuid(), Succeeded = true, Answer = "no", AgentId = "b" },
            new() { TaskId = Guid.NewGuid(), Succeeded = true, Answer = "yes", AgentId = "c" }
        };

        var result = await aggregator.AggregateAsync(results, AggregationStrategy.Vote);

        Assert.Equal("yes", result.Answer);
    }

    [Fact]
    public async Task Aggregator_AllFail_ReturnsFailure()
    {
        var aggregator = new ResultAggregator();
        var results = new List<DelegatedTaskResult>
        {
            new() { TaskId = Guid.NewGuid(), Succeeded = false, Error = "x" }
        };

        var result = await aggregator.AggregateAsync(results, AggregationStrategy.FirstSuccess);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task WorkerAgent_ExecutesTaskAndProducesAnswer()
    {
        var worker = Worker("coach", "training");

        var result = await worker.ExecuteAsync(new DelegatedTask { Goal = "Plan a training drill", AssignedAgentId = "coach" });

        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(result.Answer));
        Assert.Equal("coach", result.AgentId);
    }

    [Fact]
    public async Task SupervisorAgent_DelegatesAndAggregates()
    {
        var workers = new List<IWorkerAgent>
        {
            Worker("finance", "finance"),
            Worker("training", "training")
        };
        var supervisor = new SupervisorAgent(workers, new AgentRouter(), new ResultAggregator());

        var run = await supervisor.RunAsync(new SupervisorRunRequest
        {
            Goal = "finance: reconcile the ledger; training: schedule a session",
            Strategy = AggregationStrategy.Merge
        });

        Assert.True(run.Succeeded);
        Assert.NotEmpty(run.Results);
        Assert.False(string.IsNullOrEmpty(run.Answer));
    }

    [Fact]
    public async Task SupervisorAgent_NoWorkersReturnsFailure()
    {
        var supervisor = new SupervisorAgent([], new AgentRouter(), new ResultAggregator());

        var run = await supervisor.RunAsync(new SupervisorRunRequest { Goal = "anything" });

        Assert.False(run.Succeeded);
        Assert.Contains("No worker", run.Answer);
    }

    [Fact]
    public async Task CollaborationCoordinator_RoutesAndExecutes()
    {
        var workers = new List<IWorkerAgent>
        {
            Worker("finance", "finance"),
            Worker("training", "training")
        };
        var coordinator = new CollaborationCoordinator(workers, new AgentRouter(), new ResultAggregator());

        var run = await coordinator.CoordinateAsync(new SupervisorRunRequest
        {
            Goal = "training: prepare the batting camp",
            Strategy = AggregationStrategy.FirstSuccess
        });

        Assert.True(run.Succeeded);
        Assert.Equal("training", run.Results[0].AgentId);
    }
}
