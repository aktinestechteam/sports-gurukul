using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventVenue : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? FacilityId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Capacity { get; set; }
    public string? MapUrl { get; set; }
    public string? Instructions { get; set; }
    public bool IsPrimary { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public Facility? Facility { get; set; }
}
