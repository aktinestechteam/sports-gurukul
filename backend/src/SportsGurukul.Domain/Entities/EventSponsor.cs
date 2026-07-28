using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventSponsor : BaseEntity
{
    public Guid EventId { get; set; }
    public string SponsorName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public decimal? ContributionAmount { get; set; }
    public string? ContributionDescription { get; set; }
    public string? Tier { get; set; }
    public int DisplayOrder { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
}
