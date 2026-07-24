using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachAthlete : BaseEntity
{
    public Guid CoachId { get; set; }
    public Guid AthleteId { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Coach Coach { get; set; } = null!;
    public Athlete Athlete { get; set; } = null!;
}
