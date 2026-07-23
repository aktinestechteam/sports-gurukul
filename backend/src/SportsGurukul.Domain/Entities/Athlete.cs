using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Athlete : BaseEntity
{
    public Guid UserId { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public AthleteLevel CurrentLevel { get; set; } = AthleteLevel.Beginner;
    public int ExperienceYears { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public DominantHand? DominantHand { get; set; }
    public DominantFoot? DominantFoot { get; set; }
    public string? Biography { get; set; }
    public AthleteStatus Status { get; set; } = AthleteStatus.Active;
    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
    public MedicalProfile? MedicalProfile { get; set; }
    public EmergencyContact? EmergencyContact { get; set; }
    public Ranking? Ranking { get; set; }
    public ICollection<AthleteSport> AthleteSports { get; set; } = new List<AthleteSport>();
    public ICollection<AthleteAchievement> AthleteAchievements { get; set; } = new List<AthleteAchievement>();
}
