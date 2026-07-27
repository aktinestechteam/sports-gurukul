using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentSponsor : BaseEntity
{
    public Guid TournamentId { get; set; }
    public string SponsorName { get; set; } = string.Empty;
    public string? SponsorType { get; set; }
    public decimal? Amount { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
}
