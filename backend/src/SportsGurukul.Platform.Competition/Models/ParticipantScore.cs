namespace SportsGurukul.Platform.Competition.Models;

public class ParticipantScore
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int GamesWon { get; set; }
    public int SetsWon { get; set; }
    public int Fouls { get; set; }
    public int Timeouts { get; set; }
    public List<ScoringBreakdown> ScoringHistory { get; set; } = new();
}
