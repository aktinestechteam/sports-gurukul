using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class Participant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? TeamId { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? AthleteId { get; set; }
    public int? Ranking { get; set; }
    public string? Region { get; set; }
    public string? SeedNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBye { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
