using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TrainingMilestone : BaseEntity
{
    public Guid ProgramId { get; set; }
    public string MilestoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int WeekNumber { get; set; }
    public bool IsCompleted { get; set; }

    public TrainingProgram Program { get; set; } = null!;
}
