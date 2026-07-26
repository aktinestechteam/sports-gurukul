using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

#region Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateFacilityApiRequest"/>.
/// </summary>
public class CreateFacilityApiRequestExample : IExamplesProvider<CreateFacilityApiRequest>
{
    public CreateFacilityApiRequest GetExamples() => new()
    {
        AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        BranchId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        FacilityName = "Indoor Badminton Hall",
        FacilityType = FacilityType.BadmintonCourt,
        Description = "Professional-grade indoor badminton hall with 6 courts and world-class lighting.",
        Capacity = 120,
        IndoorOutdoor = IndoorOutdoor.Indoor,
        SurfaceType = "Synthetic",
        LightingAvailable = true,
        ParkingAvailable = true,
        ChangingRoomAvailable = true,
        WashroomAvailable = true,
        MedicalRoomAvailable = false
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateFacilityApiRequest"/>.
/// </summary>
public class UpdateFacilityApiRequestExample : IExamplesProvider<UpdateFacilityApiRequest>
{
    public UpdateFacilityApiRequest GetExamples() => new()
    {
        FacilityName = "Pro Badminton Arena",
        Description = "Renovated indoor badminton hall with 8 courts and upgraded lighting.",
        Capacity = 150,
        LightingAvailable = true,
        MedicalRoomAvailable = true,
        Status = FacilityStatus.Active
    };
}

/// <summary>
/// Swagger request example for <see cref="AddCourtApiRequest"/>.
/// </summary>
public class AddCourtApiRequestExample : IExamplesProvider<AddCourtApiRequest>
{
    public AddCourtApiRequest GetExamples() => new()
    {
        CourtNumber = "1",
        CourtName = "Court A1",
        CourtType = "Doubles",
        Capacity = 4,
        Description = "Championship-standard doubles court with sprung wooden flooring."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateCourtApiRequest"/>.
/// </summary>
public class UpdateCourtApiRequestExample : IExamplesProvider<UpdateCourtApiRequest>
{
    public UpdateCourtApiRequest GetExamples() => new()
    {
        CourtName = "Court A1 - Premium",
        CourtType = "Singles",
        Capacity = 2,
        Status = FacilityCourtStatus.Available,
        Description = "Premium singles court with tournament-grade lighting."
    };
}

/// <summary>
/// Swagger request example for <see cref="AddEquipmentApiRequest"/>.
/// </summary>
public class AddEquipmentApiRequestExample : IExamplesProvider<AddEquipmentApiRequest>
{
    public AddEquipmentApiRequest GetExamples() => new()
    {
        EquipmentName = "Yonex Astrox 88D Pro",
        Category = "Racket",
        PurchaseDate = new DateTime(2025, 3, 15),
        Condition = EquipmentCondition.New,
        MaintenanceSchedule = "Monthly inspection and restringing every 3 months",
        WarrantyExpiry = new DateTime(2027, 3, 15),
        Quantity = 20,
        Description = "Professional-grade badminton rackets for training sessions."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateEquipmentApiRequest"/>.
/// </summary>
public class UpdateEquipmentApiRequestExample : IExamplesProvider<UpdateEquipmentApiRequest>
{
    public UpdateEquipmentApiRequest GetExamples() => new()
    {
        EquipmentName = "Yonex Astrox 99 Pro",
        Category = "Premium Racket",
        Condition = EquipmentCondition.Good,
        MaintenanceSchedule = "Bi-weekly inspection",
        WarrantyExpiry = new DateTime(2028, 1, 1),
        Quantity = 25,
        Status = EquipmentStatus.Active,
        Description = "Upgraded to premium rackets for advanced training."
    };
}

/// <summary>
/// Swagger request example for <see cref="ScheduleMaintenanceApiRequest"/>.
/// </summary>
public class ScheduleMaintenanceApiRequestExample : IExamplesProvider<ScheduleMaintenanceApiRequest>
{
    public ScheduleMaintenanceApiRequest GetExamples() => new()
    {
        ScheduledDate = new DateTime(2026, 2, 15),
        MaintenanceType = "Restringing",
        Description = "Full restringing of 10 rackets with BG80 string at 24 lbs tension.",
        Cost = 5000.00m,
        PerformedBy = "SportsTech Services Pvt. Ltd.",
        Notes = "Use Yonex BG80 string. Priority equipment for upcoming tournament."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdatePricingApiRequest"/>.
/// </summary>
public class UpdatePricingApiRequestExample : IExamplesProvider<UpdatePricingApiRequest>
{
    public UpdatePricingApiRequest GetExamples() => new()
    {
        PricingName = "Standard Court Rental",
        HourlyRate = 500.00m,
        DailyRate = 3000.00m,
        MonthlyRate = 25000.00m,
        PeakHourlyRate = 750.00m,
        OffPeakHourlyRate = 350.00m,
        Description = "Standard pricing for regular court booking including basic amenities."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateScheduleApiRequest"/>.
/// </summary>
public class UpdateScheduleApiRequestExample : IExamplesProvider<UpdateScheduleApiRequest>
{
    public UpdateScheduleApiRequest GetExamples() => new()
    {
        DayOfWeek = 1,
        OpeningTime = "06:00",
        ClosingTime = "22:00",
        IsClosed = false,
        IsMaintenanceWindow = false,
        Notes = "Extended hours on match days."
    };
}

/// <summary>
/// Swagger request example for <see cref="AddFacilityImageApiRequest"/>.
/// </summary>
public class AddFacilityImageApiRequestExample : IExamplesProvider<AddFacilityImageApiRequest>
{
    public AddFacilityImageApiRequest GetExamples() => new()
    {
        ImageUrl = "https://cdn.sportsgurukul.com/facilities/badminton-hall-01.jpg",
        Caption = "Main hall view from entrance",
        IsPrimary = true
    };
}

#endregion

#region Response Examples

/// <summary>
/// Swagger response example for <see cref="FacilityDetailDto"/>.
/// </summary>
public class FacilityDetailDtoExample : IExamplesProvider<FacilityDetailDto>
{
    public FacilityDetailDto GetExamples() => new()
    {
        Id = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
        AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        BranchId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        FacilityCode = "FAC-20250615-BDM01",
        FacilityName = "Indoor Badminton Hall",
        FacilityType = "BadmintonCourt",
        Description = "Professional-grade indoor badminton hall with 6 courts and world-class lighting.",
        Capacity = 120,
        IndoorOutdoor = "Indoor",
        SurfaceType = "Synthetic",
        LightingAvailable = true,
        ParkingAvailable = true,
        ChangingRoomAvailable = true,
        WashroomAvailable = true,
        MedicalRoomAvailable = false,
        Status = "Active",
        CreatedAt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 7, 1, 14, 0, 0, DateTimeKind.Utc),
        Courts =
        [
            new CourtDto
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                CourtNumber = "1",
                CourtName = "Court A1",
                CourtType = "Doubles",
                Capacity = 4,
                Status = "Available",
                Description = "Championship-standard doubles court."
            },
            new CourtDto
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000002"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                CourtNumber = "2",
                CourtName = "Court A2",
                CourtType = "Singles",
                Capacity = 2,
                Status = "Available",
                Description = "Training court for singles practice."
            }
        ],
        Equipment =
        [
            new EquipmentDto
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                EquipmentName = "Yonex Astrox 88D Pro",
                Category = "Racket",
                PurchaseDate = new DateTime(2025, 3, 15),
                Condition = "New",
                MaintenanceSchedule = "Monthly inspection",
                WarrantyExpiry = new DateTime(2027, 3, 15),
                Quantity = 20,
                Status = "Active",
                Description = "Professional-grade badminton rackets."
            }
        ],
        Schedules =
        [
            new ScheduleDto
            {
                Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                DayOfWeek = "Monday",
                OpeningTime = "06:00",
                ClosingTime = "22:00",
                IsClosed = false,
                IsMaintenanceWindow = false,
                Notes = null
            }
        ],
        PricingTiers =
        [
            new PricingDto
            {
                Id = Guid.Parse("d2000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                PricingName = "Standard Court Rental",
                HourlyRate = 500.00m,
                DailyRate = 3000.00m,
                MonthlyRate = 25000.00m,
                PeakHourlyRate = 750.00m,
                OffPeakHourlyRate = 350.00m,
                Description = "Standard pricing for regular court booking.",
                IsActive = true
            }
        ],
        Images =
        [
            new ImageDto
            {
                Id = Guid.Parse("d3000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                ImageUrl = "https://cdn.sportsgurukul.com/facilities/badminton-hall-01.jpg",
                Caption = "Main hall view from entrance",
                IsPrimary = true,
                SortOrder = 0
            }
        ],
        Amenities =
        [
            new AmenityDto
            {
                Id = Guid.Parse("d4000000-0000-0000-0000-000000000001"),
                FacilityId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                AmenityName = "Free Wi-Fi",
                Description = "High-speed Wi-Fi throughout the facility.",
                IsAvailable = true
            }
        ]
    };
}

/// <summary>
/// Swagger response example for <see cref="FacilitySearchResponse"/>.
/// </summary>
public class FacilitySearchResponseExample : IExamplesProvider<FacilitySearchResponse>
{
    public FacilitySearchResponse GetExamples() => new()
    {
        Items =
        [
            new FacilitySummaryDto
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                FacilityCode = "FAC-20250615-BDM01",
                FacilityName = "Indoor Badminton Hall",
                FacilityType = "BadmintonCourt",
                Capacity = 120,
                IndoorOutdoor = "Indoor",
                Status = "Active",
                TotalCourts = 6,
                CreatedAt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc)
            },
            new FacilitySummaryDto
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000002"),
                AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                FacilityCode = "FAC-20250701-CRK01",
                FacilityName = "Cricket Ground",
                FacilityType = "CricketGround",
                Capacity = 500,
                IndoorOutdoor = "Outdoor",
                Status = "Active",
                TotalCourts = 2,
                CreatedAt = new DateTime(2025, 7, 1, 8, 0, 0, DateTimeKind.Utc)
            }
        ],
        TotalRecords = 12,
        TotalPages = 1,
        CurrentPage = 1,
        PageSize = 20
    };
}

#endregion
