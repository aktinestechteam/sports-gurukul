using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Planning;

namespace SportsGurukul.Platform.AI.Tests;

public class PlanningServiceTests
{
    private readonly PlanningService _service = new(NullLogger<PlanningService>.Instance);

    [Fact]
    public async Task DecomposeAsync_SplitsGoalIntoSteps()
    {
        var goal = new PlanningGoal
        {
            Description = "Analyze player performance; Schedule training; Approve the budget"
        };

        var steps = await _service.DecomposeAsync(goal);

        Assert.Equal(3, steps.Count);
        Assert.Contains(steps, s => s.Title.Contains("Analyze", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(steps, s => s.Title.Contains("Schedule", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DecomposeAsync_ExtractsToolFromStatement()
    {
        var goal = new PlanningGoal { Description = "search: find player stats" };

        var steps = await _service.DecomposeAsync(goal);

        Assert.Single(steps);
        Assert.Equal("search", steps[0].ToolName);
    }

    [Fact]
    public async Task PrioritizeAsync_OrdersByPriority()
    {
        var steps = new List<PlanStep>
        {
            new() { Title = "optional cleanup", Priority = TaskPriority.Low },
            new() { Title = "critical fix", Priority = TaskPriority.Critical }
        };

        var prioritized = await _service.PrioritizeAsync(steps);

        Assert.Equal(TaskPriority.Critical, prioritized[0].Priority);
        Assert.Equal(TaskPriority.Low, prioritized[1].Priority);
    }

    [Fact]
    public async Task CreatePlanAsync_ProducesPlanWithConfidence()
    {
        var plan = await _service.CreatePlanAsync(new PlanningGoal { Description = "Do something useful" });

        Assert.NotEqual(Guid.Empty, plan.Id);
        Assert.NotEmpty(plan.Steps);
        Assert.InRange(plan.Confidence, 0, 1);
    }

    [Fact]
    public async Task ReplanAsync_MarksFailedStep_BlocksDependents_AddsRecovery()
    {
        var stepA = new PlanStep { Id = "a", Title = "Step A", Priority = TaskPriority.High };
        var stepB = new PlanStep { Id = "b", Title = "Step B", DependsOn = ["a"], Priority = TaskPriority.Medium };
        var stepC = new PlanStep { Id = "c", Title = "Step C", Priority = TaskPriority.Low };
        var plan = new Plan { Goal = "Goal", Steps = new List<PlanStep> { stepA, stepB, stepC } };

        var replan = await _service.ReplanAsync(plan, "a", "Step A exploded");

        Assert.Equal(2, replan.Revision);
        Assert.DoesNotContain(replan.Steps, s => s.Id == "a");
        Assert.Contains(replan.Steps, s => s.Id == "b" && s.State == TaskState.Blocked);
        Assert.Contains(replan.Steps, s => s.Title.Contains("Recovery", StringComparison.OrdinalIgnoreCase));
        Assert.True(replan.Confidence < plan.Confidence);
    }

    [Fact]
    public async Task IsGoalSatisfiedAsync_TrueWhenAllStepsCompleted()
    {
        var plan = new Plan
        {
            Goal = "Goal",
            Steps = new List<PlanStep>
            {
                new() { State = TaskState.Completed },
                new() { State = TaskState.Completed }
            }
        };

        Assert.True(await _service.IsGoalSatisfiedAsync(plan));
    }

    [Fact]
    public async Task IsGoalSatisfiedAsync_FalseWhenAnyStepPending()
    {
        var plan = new Plan
        {
            Goal = "Goal",
            Steps = new List<PlanStep>
            {
                new() { State = TaskState.Completed },
                new() { State = TaskState.Pending }
            }
        };

        Assert.False(await _service.IsGoalSatisfiedAsync(plan));
    }
}
