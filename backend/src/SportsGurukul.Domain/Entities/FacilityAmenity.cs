using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilityAmenity : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Facility Facility { get; set; } = null!;
}
