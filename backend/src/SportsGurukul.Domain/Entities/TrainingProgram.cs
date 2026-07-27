using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingProgram : BaseEntity
{
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public Guid SportId { get; set; }
    public Guid AcademyId { get; set; }
    public string? Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Beginner;
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public int DurationWeeks { get; set; }
    public int Capacity { get; set; }
    public TrainingProgramStatus Status { get; set; } = TrainingProgramStatus.Draft;
    public byte[] RowVersion { get; set; } = [];

    public Sport Sport { get; set; } = null!;
    public Academy Academy { get; set; } = null!;
    public ICollection<TrainingProgramSport> ProgramSports { get; set; } = new List<TrainingProgramSport>();
    public ICollection<TrainingBatch> Batches { get; set; } = new List<TrainingBatch>();
    public ICollection<TrainingGoal> Goals { get; set; } = new List<TrainingGoal>();
    public ICollection<TrainingMilestone> Milestones { get; set; } = new List<TrainingMilestone>();
    public ICollection<TrainingMaterial> Materials { get; set; } = new List<TrainingMaterial>();
}
