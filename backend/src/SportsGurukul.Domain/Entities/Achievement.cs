using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Achievement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Competition { get; set; }
    public string? Position { get; set; }
    public AchievementLevel Level { get; set; } = AchievementLevel.Local;
    public DateTime Date { get; set; }
    public string? CertificateUrl { get; set; }

    public ICollection<AthleteAchievement> AthleteAchievements { get; set; } = new List<AthleteAchievement>();
}
