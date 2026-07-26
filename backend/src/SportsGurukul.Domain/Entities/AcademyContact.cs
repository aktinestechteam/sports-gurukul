using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademyContact : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string? PrimaryContactName { get; set; }
    public string? PrimaryPhone { get; set; }
    public string? PrimaryEmail { get; set; }
    public string? SecondaryContactName { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? SecondaryEmail { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public Academy Academy { get; set; } = null!;
}
