using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentSport : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid SportId { get; set; }
    public string? SportName { get; set; }
    public bool IsPrimary { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
}
