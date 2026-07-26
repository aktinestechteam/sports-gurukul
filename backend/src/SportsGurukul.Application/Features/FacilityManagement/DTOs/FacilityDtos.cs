namespace SportsGurukul.Application.Features.FacilityManagement.DTOs;

public class FacilityDetailDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string FacilityCode { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public string IndoorOutdoor { get; set; } = string.Empty;
    public string? SurfaceType { get; set; }
    public bool LightingAvailable { get; set; }
    public bool ParkingAvailable { get; set; }
    public bool ChangingRoomAvailable { get; set; }
    public bool WashroomAvailable { get; set; }
    public bool MedicalRoomAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<CourtDto> Courts { get; set; } = [];
    public IReadOnlyList<EquipmentDto> Equipment { get; set; } = [];
    public IReadOnlyList<ScheduleDto> Schedules { get; set; } = [];
    public IReadOnlyList<PricingDto> PricingTiers { get; set; } = [];
    public IReadOnlyList<ImageDto> Images { get; set; } = [];
    public IReadOnlyList<AmenityDto> Amenities { get; set; } = [];
}

public class FacilitySummaryDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public string FacilityCode { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string IndoorOutdoor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalCourts { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FacilitySearchResponse
{
    public IReadOnlyList<FacilitySummaryDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public class CourtDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string CourtNumber { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public string? CourtType { get; set; }
    public int? Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class EquipmentDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string? MaintenanceSchedule { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class MaintenanceDto
{
    public Guid Id { get; set; }
    public Guid FacilityEquipmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
}

public class ScheduleDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
    public bool IsMaintenanceWindow { get; set; }
    public string? Notes { get; set; }
}

public class PricingDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string PricingName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? PeakHourlyRate { get; set; }
    public decimal? OffPeakHourlyRate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class ImageDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

public class AmenityDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
