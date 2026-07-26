using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.RestoreFacility;

public class RestoreFacilityCommandHandler : IRequestHandler<RestoreFacilityCommand, Result<FacilityDetailDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreFacilityCommandHandler> _logger;

    public RestoreFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreFacilityCommandHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDetailDto>> Handle(RestoreFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<FacilityDetailDto>.Failure("Facility not found.");
        }

        if (!facility.IsDeleted)
        {
            return Result<FacilityDetailDto>.Failure("Facility is not deleted.");
        }

        facility.IsDeleted = false;
        facility.UpdatedAt = DateTime.UtcNow;

        _facilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility restored with Id: {FacilityId}", facility.Id);

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
            UpdatedAt = facility.UpdatedAt
        };

        return Result<FacilityDetailDto>.Success(dto);
    }
}
