namespace SportsGurukul.Platform.Competition.Models;

public class MatchSet
{
    public int SetNumber { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? SetDetails { get; set; }
}
