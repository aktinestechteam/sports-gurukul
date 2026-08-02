using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.MultiAgent;

public class CollaborationCoordinator : ICollaborationCoordinator
{
    private readonly IEnumerable<IWorkerAgent> _workers;
    private readonly IAgentRouter _router;
    private readonly IResultAggregator _aggregator;
    private readonly ILogger<CollaborationCoordinator> _logger;

    public CollaborationCoordinator(
        IEnumerable<IWorkerAgent> workers,
        IAgentRouter router,
        IResultAggregator aggregator,
        ILogger<CollaborationCoordinator>? logger = null)
    {
        _workers = workers;
        _router = router;
        _aggregator = aggregator;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<CollaborationCoordinator>.Instance;
    }

    public async Task<SupervisorRunResult> CoordinateAsync(SupervisorRunRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Goal))
        {
            throw new AgentPlatformException("Coordination goal is required.", "SUPERVISOR_INVALID");
        }

        var candidates = ResolveCandidates(request.WorkerAgentIds).ToList();
        if (candidates.Count == 0)
        {
            return new SupervisorRunResult
            {
                Goal = request.Goal,
                Succeeded = false,
                Answer = "No worker agents available to collaborate.",
                Strategy = request.Strategy
            };
        }

        var task = new DelegatedTask { Goal = request.Goal, Input = request.Input };
        var decision = await _router.RouteAsync(task, candidates, cancellationToken);

        IReadOnlyList<IWorkerAgent> toRun = decision.SelectedAgentId is null ? candidates : candidates.Where(w => w.Name.Equals(decision.SelectedAgentId, StringComparison.OrdinalIgnoreCase)).ToList();

        var tasks = toRun.Select(worker => new DelegatedTask
        {
            Goal = request.Goal,
            Input = request.Input,
            AssignedAgentId = worker.Name
        }).ToList();

        var results = await Task.WhenAll(tasks.Select(t => ExecuteForAsync(t, cancellationToken)));
        var aggregate = await _aggregator.AggregateAsync(results, request.Strategy, cancellationToken);

        return new SupervisorRunResult
        {
            RunId = Guid.NewGuid(),
            Goal = request.Goal,
            Succeeded = aggregate.Succeeded,
            Answer = aggregate.Answer,
            Results = results,
            Strategy = request.Strategy,
            CompletedAt = DateTime.UtcNow
        };
    }

    private async Task<DelegatedTaskResult> ExecuteForAsync(DelegatedTask task, CancellationToken cancellationToken)
    {
        var worker = _workers.FirstOrDefault(w => w.Name.Equals(task.AssignedAgentId, StringComparison.OrdinalIgnoreCase));
        if (worker is null)
        {
            return new DelegatedTaskResult
            {
                TaskId = task.TaskId,
                Goal = task.Goal,
                AgentId = task.AssignedAgentId,
                Succeeded = false,
                Error = $"Worker agent '{task.AssignedAgentId}' is not available."
            };
        }

        try
        {
            return await worker.ExecuteAsync(task, cancellationToken);
        }
        catch (Exception ex)
        {
            return new DelegatedTaskResult
            {
                TaskId = task.TaskId,
                Goal = task.Goal,
                AgentId = worker.Name,
                Succeeded = false,
                Error = ex.Message
            };
        }
    }

    private IEnumerable<IWorkerAgent> ResolveCandidates(IReadOnlyList<string>? workerAgentIds)
    {
        if (workerAgentIds is null || workerAgentIds.Count == 0)
        {
            return _workers;
        }

        var wanted = new HashSet<string>(workerAgentIds, StringComparer.OrdinalIgnoreCase);
        return _workers.Where(w => wanted.Contains(w.Name));
    }
}
