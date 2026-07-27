namespace SportsGurukul.Platform.Competition.Models;

public class LeaderboardEntry
{
    public int Position { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int MatchesPlayed { get; set; }
    public decimal WinPercentage { get; set; }
    public int GoalDifference { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public decimal AverageScore { get; set; }
    public int CurrentStreak { get; set; }
    public string? StreakType { get; set; }
    public int HeadToHeadWins { get; set; }
    public int TieBreakerValue { get; set; }
}
