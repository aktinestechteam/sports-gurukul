using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentPlanner
{
    Task<Plan> CreatePlanAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default);

    Task<Plan> ReplanAsync(Plan plan, string failedStepId, string reason, AgentContext? context = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanStep>> PrioritizeAsync(IReadOnlyList<PlanStep> steps, CancellationToken cancellationToken = default);
}
