using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Facility : BaseEntity
{
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string FacilityCode { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public FacilityType FacilityType { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public IndoorOutdoor IndoorOutdoor { get; set; }
    public string? SurfaceType { get; set; }
    public bool LightingAvailable { get; set; }
    public bool ParkingAvailable { get; set; }
    public bool ChangingRoomAvailable { get; set; }
    public bool WashroomAvailable { get; set; }
    public bool MedicalRoomAvailable { get; set; }
    public FacilityStatus Status { get; set; } = FacilityStatus.PendingApproval;
    public byte[] RowVersion { get; set; } = [];

    public Academy Academy { get; set; } = null!;
    public AcademyBranch? Branch { get; set; }
    public ICollection<FacilityArea> Areas { get; set; } = new List<FacilityArea>();
    public ICollection<FacilityCourt> Courts { get; set; } = new List<FacilityCourt>();
    public ICollection<FacilityEquipment> Equipment { get; set; } = new List<FacilityEquipment>();
    public ICollection<FacilitySchedule> Schedules { get; set; } = new List<FacilitySchedule>();
    public ICollection<FacilityPricing> PricingTiers { get; set; } = new List<FacilityPricing>();
    public ICollection<FacilityImage> Images { get; set; } = new List<FacilityImage>();
    public ICollection<FacilityAmenity> Amenities { get; set; } = new List<FacilityAmenity>();
    public ICollection<FacilityReview> Reviews { get; set; } = new List<FacilityReview>();
}
