using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentCourt : BaseEntity
{
    public Guid TournamentVenueId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public string? CourtType { get; set; }
    public int? SurfaceRating { get; set; }
    public bool IsAvailable { get; set; } = true;
    public FacilityCourtStatus Status { get; set; } = FacilityCourtStatus.Available;
    public byte[] RowVersion { get; set; } = [];

    public TournamentVenue TournamentVenue { get; set; } = null!;
    public ICollection<TournamentMatch> Matches { get; set; } = new List<TournamentMatch>();
}
