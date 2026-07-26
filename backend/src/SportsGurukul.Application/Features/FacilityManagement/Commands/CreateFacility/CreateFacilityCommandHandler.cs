using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.CreateFacility;

public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, Result<FacilityDetailDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFacilityCommandHandler> _logger;

    public CreateFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFacilityCommandHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDetailDto>> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        var isNameUnique = await _facilityRepository.IsFacilityNameUniqueInBranchAsync(
            request.AcademyId, request.BranchId, request.FacilityName, cancellationToken);

        if (!isNameUnique)
        {
            return Result<FacilityDetailDto>.Failure("A facility with this name already exists in the branch.");
        }

        var facilityCode = GenerateFacilityCode();

        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityCode = facilityCode,
            FacilityName = request.FacilityName,
            FacilityType = request.FacilityType,
            Description = request.Description,
            Capacity = request.Capacity,
            IndoorOutdoor = request.IndoorOutdoor,
            SurfaceType = request.SurfaceType,
            LightingAvailable = request.LightingAvailable,
            ParkingAvailable = request.ParkingAvailable,
            ChangingRoomAvailable = request.ChangingRoomAvailable,
            WashroomAvailable = request.WashroomAvailable,
            MedicalRoomAvailable = request.MedicalRoomAvailable
        };

        await _facilityRepository.AddAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility created with Id: {FacilityId} and Code: {FacilityCode}", facility.Id, facilityCode);

        var dto = MapToDto(facility);
        return Result<FacilityDetailDto>.Success(dto);
    }

    private static string GenerateFacilityCode()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random();
        var alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var suffix = new string(Enumerable.Range(0, 4).Select(_ => alphanumeric[random.Next(alphanumeric.Length)]).ToArray());
        return $"FAC-{datePart}-{suffix}";
    }

    private static FacilityDetailDto MapToDto(Facility facility)
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
