using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TrainingProgramSport : BaseEntity
{
    public Guid TrainingProgramId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }

    public TrainingProgram TrainingProgram { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
}
