using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for creating a new facility.
/// </summary>
public class CreateFacilityApiRequest
{
    /// <summary>Unique identifier of the academy this facility belongs to.</summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid AcademyId { get; set; }

    /// <summary>Optional branch identifier within the academy.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid? BranchId { get; set; }

    /// <summary>Name of the facility.</summary>
    /// <example>Indoor Badminton Hall</example>
    public string FacilityName { get; set; } = string.Empty;

    /// <summary>Type of the facility.</summary>
    /// <example>BadmintonCourt</example>
    public FacilityType FacilityType { get; set; }

    /// <summary>Optional description of the facility.</summary>
    /// <example>Professional-grade indoor badminton hall with 6 courts and world-class lighting.</example>
    public string? Description { get; set; }

    /// <summary>Maximum capacity of the facility.</summary>
    /// <example>120</example>
    public int Capacity { get; set; }

    /// <summary>Whether the facility is indoor, outdoor, or both.</summary>
    /// <example>Indoor</example>
    public IndoorOutdoor IndoorOutdoor { get; set; }

    /// <summary>Surface type of the facility (e.g., wooden, synthetic, grass).</summary>
    /// <example>Synthetic</example>
    public string? SurfaceType { get; set; }

    /// <summary>Whether lighting is available at the facility.</summary>
    /// <example>true</example>
    public bool LightingAvailable { get; set; }

    /// <summary>Whether parking is available at the facility.</summary>
    /// <example>true</example>
    public bool ParkingAvailable { get; set; }

    /// <summary>Whether changing rooms are available.</summary>
    /// <example>true</example>
    public bool ChangingRoomAvailable { get; set; }

    /// <summary>Whether washrooms are available.</summary>
    /// <example>true</example>
    public bool WashroomAvailable { get; set; }

    /// <summary>Whether a medical room is available.</summary>
    /// <example>false</example>
    public bool MedicalRoomAvailable { get; set; }
}

/// <summary>
/// Request body for updating a facility.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateFacilityApiRequest
{
    /// <summary>Updated facility name.</summary>
    /// <example>Pro Badminton Arena</example>
    public string? FacilityName { get; set; }

    /// <summary>Updated facility type.</summary>
    /// <example>BadmintonCourt</example>
    public FacilityType? FacilityType { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Renovated indoor badminton hall with 8 courts and upgraded lighting.</example>
    public string? Description { get; set; }

    /// <summary>Updated capacity.</summary>
    /// <example>150</example>
    public int? Capacity { get; set; }

    /// <summary>Updated indoor/outdoor classification.</summary>
    /// <example>Indoor</example>
    public IndoorOutdoor? IndoorOutdoor { get; set; }

    /// <summary>Updated surface type.</summary>
    /// <example>Wooden</example>
    public string? SurfaceType { get; set; }

    /// <summary>Updated lighting availability.</summary>
    /// <example>true</example>
    public bool? LightingAvailable { get; set; }

    /// <summary>Updated parking availability.</summary>
    /// <example>true</example>
    public bool? ParkingAvailable { get; set; }

    /// <summary>Updated changing room availability.</summary>
    /// <example>true</example>
    public bool? ChangingRoomAvailable { get; set; }

    /// <summary>Updated washroom availability.</summary>
    /// <example>true</example>
    public bool? WashroomAvailable { get; set; }

    /// <summary>Updated medical room availability.</summary>
    /// <example>true</example>
    public bool? MedicalRoomAvailable { get; set; }

    /// <summary>Updated facility status.</summary>
    /// <example>Active</example>
    public FacilityStatus? Status { get; set; }
}

/// <summary>
/// Request body for adding a court to a facility.
/// </summary>
public class AddCourtApiRequest
{
    /// <summary>Court number identifier.</summary>
    /// <example>1</example>
    public string CourtNumber { get; set; } = string.Empty;

    /// <summary>Name of the court.</summary>
    /// <example>Court A1</example>
    public string CourtName { get; set; } = string.Empty;

    /// <summary>Type of the court (e.g., singles, doubles).</summary>
    /// <example>Doubles</example>
    public string? CourtType { get; set; }

    /// <summary>Maximum number of players on the court.</summary>
    /// <example>4</example>
    public int? Capacity { get; set; }

    /// <summary>Optional description of the court.</summary>
    /// <example>Championship-standard doubles court withSprung wooden flooring.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for updating a court.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateCourtApiRequest
{
    /// <summary>Updated court name.</summary>
    /// <example>Court A1 - Premium</example>
    public string? CourtName { get; set; }

    /// <summary>Updated court type.</summary>
    /// <example>Singles</example>
    public string? CourtType { get; set; }

    /// <summary>Updated capacity.</summary>
    /// <example>2</example>
    public int? Capacity { get; set; }

    /// <summary>Updated court status.</summary>
    /// <example>Available</example>
    public FacilityCourtStatus? Status { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Premium singles court with tournament-grade lighting.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for adding equipment to a facility.
/// </summary>
public class AddEquipmentApiRequest
{
    /// <summary>Name of the equipment.</summary>
    /// <example>Yonex Astrox 88D Pro</example>
    public string EquipmentName { get; set; } = string.Empty;

    /// <summary>Equipment category.</summary>
    /// <example>Racket</example>
    public string? Category { get; set; }

    /// <summary>Date the equipment was purchased.</summary>
    /// <example>2025-03-15</example>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>Current condition of the equipment.</summary>
    /// <example>New</example>
    public EquipmentCondition Condition { get; set; }

    /// <summary>Maintenance schedule description.</summary>
    /// <example>Monthly inspection and restringing every 3 months</example>
    public string? MaintenanceSchedule { get; set; }

    /// <summary>Warranty expiry date.</summary>
    /// <example>2027-03-15</example>
    public DateTime? WarrantyExpiry { get; set; }

    /// <summary>Quantity of equipment.</summary>
    /// <example>20</example>
    public int Quantity { get; set; } = 1;

    /// <summary>Optional description.</summary>
    /// <example>Professional-grade badminton rackets for training sessions.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for updating equipment.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateEquipmentApiRequest
{
    /// <summary>Updated equipment name.</summary>
    /// <example>Yonex Astrox 99 Pro</example>
    public string? EquipmentName { get; set; }

    /// <summary>Updated category.</summary>
    /// <example>Premium Racket</example>
    public string? Category { get; set; }

    /// <summary>Updated condition.</summary>
    /// <example>Good</example>
    public EquipmentCondition? Condition { get; set; }

    /// <summary>Updated maintenance schedule.</summary>
    /// <example>Bi-weekly inspection</example>
    public string? MaintenanceSchedule { get; set; }

    /// <summary>Updated warranty expiry.</summary>
    /// <example>2028-01-01</example>
    public DateTime? WarrantyExpiry { get; set; }

    /// <summary>Updated quantity.</summary>
    /// <example>25</example>
    public int? Quantity { get; set; }

    /// <summary>Updated equipment status.</summary>
    /// <example>Active</example>
    public EquipmentStatus? Status { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Upgraded to premium rackets for advanced training.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for scheduling maintenance for equipment.
/// </summary>
public class ScheduleMaintenanceApiRequest
{
    /// <summary>Date when maintenance is scheduled.</summary>
    /// <example>2026-02-15</example>
    public DateTime ScheduledDate { get; set; }

    /// <summary>Type of maintenance to be performed.</summary>
    /// <example>Restringing</example>
    public string MaintenanceType { get; set; } = string.Empty;

    /// <summary>Description of the maintenance work.</summary>
    /// <example>Full restringing of 10 rackets with BG80 string at 24 lbs tension.</example>
    public string? Description { get; set; }

    /// <summary>Estimated cost of maintenance.</summary>
    /// <example>5000.00</example>
    public decimal? Cost { get; set; }

    /// <summary>Name of the person or vendor performing maintenance.</summary>
    /// <example>SportsTech Services Pvt. Ltd.</example>
    public string? PerformedBy { get; set; }

    /// <summary>Additional notes for the maintenance.</summary>
    /// <example>Use Yonex BG80 string. Priority equipment for upcoming tournament.</example>
    public string? Notes { get; set; }
}

/// <summary>
/// Request body for updating facility pricing.
/// </summary>
public class UpdatePricingApiRequest
{
    /// <summary>Name of the pricing tier.</summary>
    /// <example>Standard Court Rental</example>
    public string PricingName { get; set; } = string.Empty;

    /// <summary>Hourly rental rate.</summary>
    /// <example>500.00</example>
    public decimal HourlyRate { get; set; }

    /// <summary>Daily rental rate.</summary>
    /// <example>3000.00</example>
    public decimal DailyRate { get; set; }

    /// <summary>Monthly rental rate.</summary>
    /// <example>25000.00</example>
    public decimal MonthlyRate { get; set; }

    /// <summary>Peak hours hourly rate.</summary>
    /// <example>750.00</example>
    public decimal? PeakHourlyRate { get; set; }

    /// <summary>Off-peak hours hourly rate.</summary>
    /// <example>350.00</example>
    public decimal? OffPeakHourlyRate { get; set; }

    /// <summary>Optional description of the pricing tier.</summary>
    /// <example>Standard pricing for regular court booking including basic amenities.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for updating a facility schedule entry.
/// </summary>
public class UpdateScheduleApiRequest
{
    /// <summary>Day of the week (0 = Sunday, 6 = Saturday).</summary>
    /// <example>1</example>
    public int DayOfWeek { get; set; }

    /// <summary>Opening time in HH:mm format.</summary>
    /// <example>06:00</example>
    public string OpeningTime { get; set; } = string.Empty;

    /// <summary>Closing time in HH:mm format.</summary>
    /// <example>22:00</example>
    public string ClosingTime { get; set; } = string.Empty;

    /// <summary>Whether the facility is closed on this day.</summary>
    /// <example>false</example>
    public bool IsClosed { get; set; }

    /// <summary>Whether this is a maintenance window.</summary>
    /// <example>false</example>
    public bool IsMaintenanceWindow { get; set; }

    /// <summary>Optional notes for this schedule entry.</summary>
    /// <example>Extended hours on match days.</example>
    public string? Notes { get; set; }
}

/// <summary>
/// Request body for adding an image to a facility.
/// </summary>
public class AddFacilityImageApiRequest
{
    /// <summary>URL of the image.</summary>
    /// <example>https://cdn.sportsgurukul.com/facilities/badminton-hall-01.jpg</example>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>Optional caption for the image.</summary>
    /// <example>Main hall view from entrance</example>
    public string? Caption { get; set; }

    /// <summary>Whether this is the primary facility image.</summary>
    /// <example>true</example>
    public bool IsPrimary { get; set; }
}
