using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TrainingGoal : BaseEntity
{
    public Guid ProgramId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TargetWeek { get; set; }
    public bool IsAchieved { get; set; }

    public TrainingProgram Program { get; set; } = null!;
}
