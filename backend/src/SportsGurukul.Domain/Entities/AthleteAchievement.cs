using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AthleteAchievement : BaseEntity
{
    public Guid AthleteId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime AwardedDate { get; set; }
    public string? Notes { get; set; }

    public Athlete Athlete { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
