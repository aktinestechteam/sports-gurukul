using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.MultiAgent;

public class SupervisorAgent : ISupervisorAgent
{
    private readonly IEnumerable<IWorkerAgent> _workers;
    private readonly IAgentRouter _router;
    private readonly IResultAggregator _aggregator;
    private readonly ILogger<SupervisorAgent> _logger;

    public SupervisorAgent(
        IEnumerable<IWorkerAgent> workers,
        IAgentRouter router,
        IResultAggregator aggregator,
        ILogger<SupervisorAgent>? logger = null)
    {
        _workers = workers;
        _router = router;
        _aggregator = aggregator;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<SupervisorAgent>.Instance;
    }

    public async Task<SupervisorRunResult> RunAsync(SupervisorRunRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Goal))
        {
            throw new AgentPlatformException("Supervisor goal is required.", "SUPERVISOR_INVALID");
        }

        var candidates = ResolveCandidates(request.WorkerAgentIds).ToList();
        if (candidates.Count == 0)
        {
            return new SupervisorRunResult
            {
                Goal = request.Goal,
                Succeeded = false,
                Answer = "No worker agents available to delegate.",
                Strategy = request.Strategy
            };
        }

        var tasks = new List<DelegatedTask>();
        foreach (var worker in candidates)
        {
            var task = new DelegatedTask
            {
                Goal = request.Goal,
                Input = request.Input,
                AssignedAgentId = worker.Name
            };

            if (!request.DelegateAllSteps)
            {
                var decision = await _router.RouteAsync(task, candidates, cancellationToken);
                if (decision.SelectedAgentId is null)
                {
                    _logger.LogWarning("Router could not find a capable worker for task '{TaskId}'", task.TaskId);
                    continue;
                }

                task.AssignedAgentId = decision.SelectedAgentId;
            }

            tasks.Add(task);
        }

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
