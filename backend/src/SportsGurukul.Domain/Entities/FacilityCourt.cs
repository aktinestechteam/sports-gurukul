using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class FacilityCourt : BaseEntity
{
    public Guid FacilityId { get; set; }
    public Guid? FacilityAreaId { get; set; }
    public string CourtNumber { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public string? CourtType { get; set; }
    public int? Capacity { get; set; }
    public FacilityCourtStatus Status { get; set; } = FacilityCourtStatus.Available;
    public string? Description { get; set; }

    public Facility Facility { get; set; } = null!;
    public FacilityArea? FacilityArea { get; set; }
}
