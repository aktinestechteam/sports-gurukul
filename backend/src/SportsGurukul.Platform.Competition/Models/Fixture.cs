using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class Fixture
{
    public Guid Id { get; set; }
    public int FixtureNumber { get; set; }
    public Guid TournamentId { get; set; }
    public Guid? StageId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? CourtId { get; set; }
    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
    public Guid? HomeParticipantId { get; set; }
    public Guid? AwayParticipantId { get; set; }
    public bool IsPublished { get; set; }
    public string? Notes { get; set; }
}
