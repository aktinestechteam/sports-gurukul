using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class FixtureDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid? TournamentStageId { get; set; }
    public string? StageName { get; set; }
    public int FixtureNumber { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public string? VenueName { get; set; }
    public string? CourtName { get; set; }
    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
    public bool IsPublished { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
