namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class StandingsDto
{
    public Guid TournamentId { get; set; }
    public List<StandingsEntryDto> Entries { get; set; } = new();
}

public class StandingsEntryDto
{
    public int Position { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int Points { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Lost { get; set; }
    public int Drawn { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
}
