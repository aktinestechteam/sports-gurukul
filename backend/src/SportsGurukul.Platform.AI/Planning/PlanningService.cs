using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Planning;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Planning;

public class PlanningService : IPlanningService
{
    private readonly ILogger<PlanningService> _logger;

    public PlanningService(ILogger<PlanningService>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<PlanningService>.Instance;
    }

    public async Task<Plan> CreatePlanAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default)
    {
        var steps = await DecomposeAsync(goal, context, cancellationToken);
        var prioritized = await PrioritizeAsync(steps, cancellationToken);

        return new Plan
        {
            Goal = goal.Description,
            Steps = prioritized,
            Confidence = EstimateConfidence(goal)
        };
    }

    public Task<IReadOnlyList<PlanStep>> DecomposeAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var statements = SplitStatements(goal.Description);
        var steps = new List<PlanStep>();

        foreach (var statement in statements)
        {
            if (string.IsNullOrWhiteSpace(statement))
            {
                continue;
            }

            var step = new PlanStep
            {
                Title = statement.Trim(),
                Description = statement.Trim(),
                Arguments = new Dictionary<string, object?>
                {
                    ["goal"] = goal.Description,
                    ["input"] = goal.Input
                }
            };

            if (TryExtractTool(statement, out var toolName))
            {
                step.ToolName = toolName;
                step.Arguments["tool"] = toolName;
            }

            steps.Add(step);
        }

        if (steps.Count == 0)
        {
            steps.Add(new PlanStep
            {
                Title = goal.Description,
                Description = goal.Description,
                Arguments = new Dictionary<string, object?> { ["goal"] = goal.Description }
            });
        }

        return Task.FromResult<IReadOnlyList<PlanStep>>(steps);
    }

    public Task<IReadOnlyList<PlanStep>> PrioritizeAsync(IEnumerable<PlanStep> steps, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ranked = steps.Select(s => new PlanStep
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            ToolName = s.ToolName,
            Arguments = s.Arguments,
            DependsOn = s.DependsOn,
            State = s.State,
            RequiresApproval = s.RequiresApproval,
            Result = s.Result,
            Priority = AssignPriority(s.Title)
        }).OrderBy(s => s.Priority).ToList();

        return Task.FromResult<IReadOnlyList<PlanStep>>(ranked);
    }

    public Task<Plan> ReplanAsync(Plan plan, string failedStepId, string reason, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var failed = plan.Steps.FirstOrDefault(s => s.Id == failedStepId);
        var updated = plan.Steps.ToList();

        if (failed is not null)
        {
            failed.State = TaskState.Failed;
            updated = updated.Where(s => s.Id != failedStepId).ToList();

            foreach (var step in updated.Where(s => s.DependsOn.Contains(failedStepId)))
            {
                step.State = TaskState.Blocked;
            }

            updated.Add(new PlanStep
            {
                Title = $"Recovery: {reason}",
                Description = $"Recovery step after failure of '{failed.Title}'. Reason: {reason}",
                Arguments = new Dictionary<string, object?> { ["reason"] = reason, ["failedStepId"] = failedStepId },
                Priority = TaskPriority.High
            });
        }

        return Task.FromResult(new Plan
        {
            Id = plan.Id,
            Goal = plan.Goal,
            Steps = updated,
            Confidence = Math.Max(0, plan.Confidence - 0.15),
            Revision = plan.Revision + 1,
            ReplanReason = reason,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<bool> IsGoalSatisfiedAsync(Plan plan, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(plan.Steps.Count > 0 && plan.Steps.All(s => s.State == TaskState.Completed));
    }

    private static IReadOnlyList<string> SplitStatements(string goal) =>
        goal.Split(['\n', '\r', ';', '.'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool TryExtractTool(string statement, out string toolName)
    {
        var colon = statement.IndexOf(':');
        if (colon > 0)
        {
            var candidate = statement[..colon].Trim();
            if (candidate.Split(' ').Length == 1)
            {
                toolName = candidate;
                return true;
            }
        }

        const string useMarker = "use ";
        if (statement.StartsWith(useMarker, StringComparison.OrdinalIgnoreCase))
        {
            var rest = statement[useMarker.Length..].Trim();
            var next = rest.Split([' ', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(next))
            {
                toolName = next;
                return true;
            }
        }

        toolName = string.Empty;
        return false;
    }

    private static TaskPriority AssignPriority(string title)
    {
        if (title.Contains("critical", StringComparison.OrdinalIgnoreCase) ||
            title.Contains("urgent", StringComparison.OrdinalIgnoreCase) ||
            title.Contains("must", StringComparison.OrdinalIgnoreCase))
        {
            return TaskPriority.Critical;
        }

        if (title.Contains("high", StringComparison.OrdinalIgnoreCase) ||
            title.Contains("approve", StringComparison.OrdinalIgnoreCase))
        {
            return TaskPriority.High;
        }

        if (title.Contains("optional", StringComparison.OrdinalIgnoreCase) ||
            title.Contains("low", StringComparison.OrdinalIgnoreCase))
        {
            return TaskPriority.Low;
        }

        return TaskPriority.Medium;
    }

    private static double EstimateConfidence(PlanningGoal goal)
    {
        var score = 1.0;
        if (string.IsNullOrWhiteSpace(goal.Description))
        {
            score -= 0.5;
        }

        if (goal.AcceptanceCriteria is not null && goal.AcceptanceCriteria.Count > 0)
        {
            score -= 0.05 * goal.AcceptanceCriteria.Count;
        }

        return Math.Clamp(score, 0.1, 1.0);
    }
}
