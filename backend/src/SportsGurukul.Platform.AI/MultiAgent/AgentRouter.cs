using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.MultiAgent;

public class AgentRouter : IAgentRouter
{
    private readonly ILogger<AgentRouter> _logger;

    public AgentRouter(ILogger<AgentRouter>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentRouter>.Instance;
    }

    public Task<AgentRoutingDecision> RouteAsync(DelegatedTask task, IReadOnlyList<IWorkerAgent> workers, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (task.AssignedAgentId is not null)
        {
            var direct = workers.FirstOrDefault(w => w.Name.Equals(task.AssignedAgentId, StringComparison.OrdinalIgnoreCase));
            if (direct is not null)
            {
                return Task.FromResult(new AgentRoutingDecision
                {
                    TaskId = task.TaskId,
                    SelectedAgentId = direct.Name,
                    Reason = "Explicit assignment",
                    Confidence = 1.0
                });
            }
        }

        var corpus = $"{task.Goal} {task.Input}".ToLowerInvariant();
        var tokens = corpus.Split([' ', ',', '.', ';', ':', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        var tokenSet = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);

        IWorkerAgent? best = null;
        var bestScore = 0;
        foreach (var worker in workers)
        {
            var score = 0;
            foreach (var capability in worker.Capabilities)
            {
                var key = capability.ToLowerInvariant();
                if (tokenSet.Contains(key))
                {
                    score += 3;
                }
                else if (corpus.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1;
                }
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = worker;
            }
        }

        if (best is null || bestScore == 0)
        {
            return Task.FromResult(new AgentRoutingDecision
            {
                TaskId = task.TaskId,
                SelectedAgentId = null,
                Reason = "No worker capability matched the task",
                Confidence = 0
            });
        }

        return Task.FromResult(new AgentRoutingDecision
        {
            TaskId = task.TaskId,
            SelectedAgentId = best.Name,
            Reason = $"Matched capabilities with score {bestScore}",
            Confidence = Math.Min(0.5 + (bestScore * 0.1), 0.95)
        });
    }
}
