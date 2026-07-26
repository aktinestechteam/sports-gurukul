using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EquipmentMaintenance : BaseEntity
{
    public Guid FacilityEquipmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }

    public FacilityEquipment FacilityEquipment { get; set; } = null!;
}
