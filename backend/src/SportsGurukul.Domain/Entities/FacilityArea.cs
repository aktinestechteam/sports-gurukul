using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilityArea : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public string? AreaType { get; set; }
    public bool IsActive { get; set; } = true;

    public Facility Facility { get; set; } = null!;
    public ICollection<FacilityCourt> Courts { get; set; } = new List<FacilityCourt>();
}
