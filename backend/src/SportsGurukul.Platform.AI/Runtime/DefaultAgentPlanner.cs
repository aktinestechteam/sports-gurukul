using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Interfaces.Planning;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Runtime;

public class DefaultAgentPlanner : IAgentPlanner
{
    private readonly IPlanningService _planning;
    private readonly IToolRegistry _toolRegistry;
    private readonly ILanguageModel? _model;
    private readonly ILogger<DefaultAgentPlanner> _logger;

    public DefaultAgentPlanner(
        IPlanningService planning,
        IToolRegistry toolRegistry,
        ILanguageModel? model = null,
        ILogger<DefaultAgentPlanner>? logger = null)
    {
        _planning = planning;
        _toolRegistry = toolRegistry;
        _model = model;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultAgentPlanner>.Instance;
    }

    public async Task<Plan> CreatePlanAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default)
    {
        var plan = await _planning.CreatePlanAsync(goal, context, cancellationToken);
        var steps = plan.Steps.ToList();
        await AssignToolsAsync(steps, context, cancellationToken);

        return new Plan
        {
            Id = plan.Id,
            Goal = plan.Goal,
            Steps = steps,
            Confidence = plan.Confidence,
            Revision = plan.Revision
        };
    }

    public async Task<Plan> ReplanAsync(Plan plan, string failedStepId, string reason, AgentContext? context = null, CancellationToken cancellationToken = default)
    {
        var replan = await _planning.ReplanAsync(plan, failedStepId, reason, cancellationToken);
        var steps = replan.Steps.ToList();
        await AssignToolsAsync(steps, context, cancellationToken);

        return new Plan
        {
            Id = replan.Id,
            Goal = replan.Goal,
            Steps = steps,
            Confidence = replan.Confidence,
            Revision = replan.Revision,
            ReplanReason = replan.ReplanReason,
            CreatedAt = replan.CreatedAt
        };
    }

    public async Task<IReadOnlyList<PlanStep>> PrioritizeAsync(IReadOnlyList<PlanStep> steps, CancellationToken cancellationToken = default) =>
        await _planning.PrioritizeAsync(steps, cancellationToken);

    private async Task AssignToolsAsync(List<PlanStep> steps, AgentContext? context, CancellationToken cancellationToken)
    {
        var tools = await _toolRegistry.GetAllAsync(cancellationToken);
        if (tools.Count == 0)
        {
            return;
        }

        var allowed = context?.Definition.AllowedToolNames ?? [];
        foreach (var step in steps)
        {
            if (!string.IsNullOrWhiteSpace(step.ToolName))
            {
                var byName = tools.FirstOrDefault(t => t.Name.Equals(step.ToolName, StringComparison.OrdinalIgnoreCase));
                if (byName is not null)
                {
                    step.RequiresApproval = byName.RequiresApproval;
                    FillMissingArguments(step, byName);
                }

                continue;
            }

            var match = SelectTool(step, tools, allowed);
            if (match is not null)
            {
                step.ToolName = match.Name;
                step.RequiresApproval = match.RequiresApproval;
                step.Arguments["tool"] = match.Name;
                FillMissingArguments(step, match);
            }
        }
    }

    private static void FillMissingArguments(PlanStep step, ITool tool)
    {
        if (step.Arguments is null)
        {
            return;
        }

        foreach (var parameter in tool.Parameters.Keys)
        {
            if (!step.Arguments.ContainsKey(parameter))
            {
                step.Arguments[parameter] = step.Title;
            }
        }
    }

    private static ITool? SelectTool(PlanStep step, IReadOnlyList<ITool> tools, IReadOnlyList<string> allowed)
    {
        ITool? best = null;
        var bestScore = 0;

        foreach (var tool in tools)
        {
            if (allowed.Count > 0 && !allowed.Contains(tool.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var score = MatchScore(step.Title, tool);
            if (score > bestScore)
            {
                bestScore = score;
                best = tool;
            }
        }

        return bestScore > 0 ? best : null;
    }

    private static int MatchScore(string title, ITool tool)
    {
        var score = 0;
        if (title.Contains(tool.Name, StringComparison.OrdinalIgnoreCase))
        {
            score += 3;
        }

        foreach (var tag in tool.Tags)
        {
            if (title.Contains(tag, StringComparison.OrdinalIgnoreCase))
            {
                score += 2;
            }
        }

        if (tool.Description is not null && title.Contains(tool.Description, StringComparison.OrdinalIgnoreCase))
        {
            score += 2;
        }

        return score;
    }
}
