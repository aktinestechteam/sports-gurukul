using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AthleteSport : BaseEntity
{
    public Guid AthleteId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
    public DateTime JoinedDate { get; set; }

    public Athlete Athlete { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
}
