using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachSport : BaseEntity
{
    public Guid CoachId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
    public DateTime JoinedDate { get; set; }

    public Coach Coach { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
}
