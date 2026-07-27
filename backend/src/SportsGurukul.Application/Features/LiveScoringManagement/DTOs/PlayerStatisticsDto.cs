namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class PlayerStatisticsDto
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
    public List<MatchPerformanceDto> RecentPerformances { get; set; } = new();
}

public class MatchPerformanceDto
{
    public Guid MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public int PointsScored { get; set; }
    public bool IsWin { get; set; }
    public bool IsDraw { get; set; }
}
