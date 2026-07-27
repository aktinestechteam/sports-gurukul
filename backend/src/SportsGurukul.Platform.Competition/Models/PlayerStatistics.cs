namespace SportsGurukul.Platform.Competition.Models;

public class PlayerStatistics
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? SportCode { get; set; }
    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public decimal WinPercentage { get; set; }
    public int TotalPoints { get; set; }
    public decimal AveragePointsPerMatch { get; set; }
    public int BestScore { get; set; }
    public int WorstScore { get; set; }
    public int CurrentStreak { get; set; }
    public string? StreakType { get; set; }
    public int LongestWinStreak { get; set; }
    public int LongestLosingStreak { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public List<MatchPerformance> RecentPerformances { get; set; } = new();
}

public class MatchPerformance
{
    public Guid MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public int PointsScored { get; set; }
    public bool IsWin { get; set; }
    public bool IsDraw { get; set; }
}
