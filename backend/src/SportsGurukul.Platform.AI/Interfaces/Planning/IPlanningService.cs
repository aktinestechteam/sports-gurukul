using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Planning;

public interface IPlanningService
{
    Task<Plan> CreatePlanAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanStep>> DecomposeAsync(PlanningGoal goal, AgentContext? context = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanStep>> PrioritizeAsync(IEnumerable<PlanStep> steps, CancellationToken cancellationToken = default);

    Task<Plan> ReplanAsync(Plan plan, string failedStepId, string reason, CancellationToken cancellationToken = default);

    Task<bool> IsGoalSatisfiedAsync(Plan plan, CancellationToken cancellationToken = default);
}
