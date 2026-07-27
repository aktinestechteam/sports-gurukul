using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentVenue : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid FacilityId { get; set; }
    public string? VenueName { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public Facility Facility { get; set; } = null!;
    public ICollection<TournamentCourt> Courts { get; set; } = new List<TournamentCourt>();
}
