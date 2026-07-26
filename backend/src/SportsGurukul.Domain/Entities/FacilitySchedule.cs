using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilitySchedule : BaseEntity
{
    public Guid FacilityId { get; set; }
    public Guid? FacilityCourtId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
    public bool IsMaintenanceWindow { get; set; }
    public string? Notes { get; set; }

    public Facility Facility { get; set; } = null!;
    public FacilityCourt? FacilityCourt { get; set; }
}
