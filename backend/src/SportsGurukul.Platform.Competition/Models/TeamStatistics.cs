namespace SportsGurukul.Platform.Competition.Models;

public class TeamStatistics
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public decimal WinPercentage { get; set; }
    public int TotalPointsFor { get; set; }
    public int TotalPointsAgainst { get; set; }
    public decimal AveragePointsPerMatch { get; set; }
    public int CurrentStreak { get; set; }
    public string? StreakType { get; set; }
    public int HomeWins { get; set; }
    public int AwayWins { get; set; }
    public List<PlayerStatistics> TopPerformers { get; set; } = new();
}
