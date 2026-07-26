using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class FacilityEquipment : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;
    public string? MaintenanceSchedule { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public int Quantity { get; set; } = 1;
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    public string? Description { get; set; }

    public Facility Facility { get; set; } = null!;
    public ICollection<EquipmentMaintenance> MaintenanceRecords { get; set; } = new List<EquipmentMaintenance>();
}
