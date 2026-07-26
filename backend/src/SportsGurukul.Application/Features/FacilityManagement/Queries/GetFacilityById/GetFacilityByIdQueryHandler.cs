using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityById;

public class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, Result<FacilityDetailDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ILogger<GetFacilityByIdQueryHandler> _logger;

    public GetFacilityByIdQueryHandler(
        IFacilityRepository facilityRepository,
        ILogger<GetFacilityByIdQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _logger = logger;
    }

    public async Task<Result<FacilityDetailDto>> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetWithDetailsAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<FacilityDetailDto>.Failure("Facility not found.");
        }

        var dto = new FacilityDetailDto
        {
            Id = facility.Id,
            AcademyId = facility.AcademyId,
            BranchId = facility.BranchId,
            FacilityCode = facility.FacilityCode,
            FacilityName = facility.FacilityName,
            FacilityType = facility.FacilityType.ToString(),
            Description = facility.Description,
            Capacity = facility.Capacity,
            IndoorOutdoor = facility.IndoorOutdoor.ToString(),
            SurfaceType = facility.SurfaceType,
            LightingAvailable = facility.LightingAvailable,
            ParkingAvailable = facility.ParkingAvailable,
            ChangingRoomAvailable = facility.ChangingRoomAvailable,
            WashroomAvailable = facility.WashroomAvailable,
            MedicalRoomAvailable = facility.MedicalRoomAvailable,
            Status = facility.Status.ToString(),
            CreatedAt = facility.CreatedAt,
            UpdatedAt = facility.UpdatedAt,
            Courts = facility.Courts.Select(c => new CourtDto
            {
                Id = c.Id,
                FacilityId = c.FacilityId,
                CourtNumber = c.CourtNumber,
                CourtName = c.CourtName,
                CourtType = c.CourtType,
                Capacity = c.Capacity,
                Status = c.Status.ToString(),
                Description = c.Description
            }).ToList(),
            Equipment = facility.Equipment.Select(e => new EquipmentDto
            {
                Id = e.Id,
                FacilityId = e.FacilityId,
                EquipmentName = e.EquipmentName,
                Category = e.Category,
                PurchaseDate = e.PurchaseDate,
                Condition = e.Condition.ToString(),
                MaintenanceSchedule = e.MaintenanceSchedule,
                WarrantyExpiry = e.WarrantyExpiry,
                Quantity = e.Quantity,
                Status = e.Status.ToString(),
                Description = e.Description
            }).ToList(),
            Schedules = facility.Schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                FacilityId = s.FacilityId,
                DayOfWeek = s.DayOfWeek.ToString(),
                OpeningTime = s.OpeningTime,
                ClosingTime = s.ClosingTime,
                IsClosed = s.IsClosed,
                IsMaintenanceWindow = s.IsMaintenanceWindow,
                Notes = s.Notes
            }).ToList(),
            PricingTiers = facility.PricingTiers.Select(p => new PricingDto
            {
                Id = p.Id,
                FacilityId = p.FacilityId,
                PricingName = p.PricingName,
                HourlyRate = p.HourlyRate,
                DailyRate = p.DailyRate,
                MonthlyRate = p.MonthlyRate,
                PeakHourlyRate = p.PeakHourlyRate,
                OffPeakHourlyRate = p.OffPeakHourlyRate,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList(),
            Images = facility.Images.Select(i => new ImageDto
            {
                Id = i.Id,
                FacilityId = i.FacilityId,
                ImageUrl = i.ImageUrl,
                Caption = i.Caption,
                IsPrimary = i.IsPrimary,
                SortOrder = i.SortOrder
            }).ToList()
        };

        _logger.LogInformation("Facility retrieved with Id: {FacilityId}", request.FacilityId);

        return Result<FacilityDetailDto>.Success(dto);
    }
}
