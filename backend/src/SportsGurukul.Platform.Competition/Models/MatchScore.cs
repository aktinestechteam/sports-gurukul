using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class MatchScore
{
    public int TotalPoints { get; set; }
    public int Games { get; set; }
    public int Sets { get; set; }
    public int Periods { get; set; }
    public int Quarters { get; set; }
    public int Halves { get; set; }
    public int Innings { get; set; }
    public int Laps { get; set; }
    public List<PeriodScore> PeriodScores { get; set; } = new();
    public List<ScoringBreakdown> Breakdown { get; set; } = new();
}

public class PeriodScore
{
    public int PeriodNumber { get; set; }
    public string? PeriodName { get; set; }
    public int Score { get; set; }
    public ScoringUnit Unit { get; set; }
}

public class ScoringBreakdown
{
    public ScoringUnit Unit { get; set; }
    public int Value { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}
