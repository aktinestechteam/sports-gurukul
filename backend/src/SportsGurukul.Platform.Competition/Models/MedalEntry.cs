using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class MedalEntry
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int GoldCount { get; set; }
    public int SilverCount { get; set; }
    public int BronzeCount { get; set; }
    public int TotalMedals { get; set; }
    public int TotalPoints { get; set; }
    public List<MedalDetail> Medals { get; set; } = new();
}

public class MedalDetail
{
    public MedalType Type { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string SportCode { get; set; } = string.Empty;
    public DateTime AchievedAt { get; set; }
}
