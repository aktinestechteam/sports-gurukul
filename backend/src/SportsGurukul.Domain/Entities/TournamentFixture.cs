using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentFixture : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? TournamentStageId { get; set; }
    public int FixtureNumber { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? CourtId { get; set; }
    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
    public bool IsPublished { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentStage? TournamentStage { get; set; }
    public Facility? Venue { get; set; }
    public FacilityCourt? Court { get; set; }
}
