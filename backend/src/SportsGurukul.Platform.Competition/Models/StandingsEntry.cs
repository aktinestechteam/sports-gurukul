namespace SportsGurukul.Platform.Competition.Models;

public class StandingsEntry
{
    public int Position { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Lost { get; set; }
    public int Drawn { get; set; }
    public int Points { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public decimal AverageGoalsPerMatch { get; set; }
    public int Form { get; set; }
}
