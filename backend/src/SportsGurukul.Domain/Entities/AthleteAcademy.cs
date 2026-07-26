using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AthleteAcademy : BaseEntity
{
    public Guid AthleteId { get; set; }
    public Guid AcademyId { get; set; }
    public DateTime RegisteredDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Athlete Athlete { get; set; } = null!;
    public Academy Academy { get; set; } = null!;
}
