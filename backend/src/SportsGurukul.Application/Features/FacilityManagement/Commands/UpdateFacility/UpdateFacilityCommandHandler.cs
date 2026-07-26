using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacility;

public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, Result<FacilityDetailDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFacilityCommandHandler> _logger;

    public UpdateFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFacilityCommandHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDetailDto>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<FacilityDetailDto>.Failure("Facility not found.");
        }

        if (request.FacilityName is not null)
            facility.FacilityName = request.FacilityName;
        if (request.FacilityType is not null)
            facility.FacilityType = request.FacilityType.Value;
        if (request.Description is not null)
            facility.Description = request.Description;
        if (request.Capacity is not null)
            facility.Capacity = request.Capacity.Value;
        if (request.IndoorOutdoor is not null)
            facility.IndoorOutdoor = request.IndoorOutdoor.Value;
        if (request.SurfaceType is not null)
            facility.SurfaceType = request.SurfaceType;
        if (request.LightingAvailable is not null)
            facility.LightingAvailable = request.LightingAvailable.Value;
        if (request.ParkingAvailable is not null)
            facility.ParkingAvailable = request.ParkingAvailable.Value;
        if (request.ChangingRoomAvailable is not null)
            facility.ChangingRoomAvailable = request.ChangingRoomAvailable.Value;
        if (request.WashroomAvailable is not null)
            facility.WashroomAvailable = request.WashroomAvailable.Value;
        if (request.MedicalRoomAvailable is not null)
            facility.MedicalRoomAvailable = request.MedicalRoomAvailable.Value;
        if (request.Status is not null)
            facility.Status = request.Status.Value;

        facility.UpdatedAt = DateTime.UtcNow;

        _facilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility updated with Id: {FacilityId}", facility.Id);

        var dto = MapToDto(facility);
        return Result<FacilityDetailDto>.Success(dto);
    }

    private static FacilityDetailDto MapToDto(Domain.Entities.Facility facility)
    {
        return new FacilityDetailDto
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
            UpdatedAt = facility.UpdatedAt
        };
    }
}
