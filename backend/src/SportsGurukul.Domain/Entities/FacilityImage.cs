using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilityImage : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public Facility Facility { get; set; } = null!;
}
