using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateFacility;

public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, Result<FacilityDto>>
{
    private readonly IAcademyFacilityRepository _academyFacilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFacilityCommandHandler> _logger;

    public UpdateFacilityCommandHandler(
        IAcademyFacilityRepository academyFacilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFacilityCommandHandler> logger)
    {
        _academyFacilityRepository = academyFacilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDto>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating facility with Id: {FacilityId}", request.FacilityId);

        var facility = await _academyFacilityRepository.GetByIdAsync(request.FacilityId);
        if (facility is null)
            return Result<FacilityDto>.Failure("Facility not found.");

        if (facility.AcademyId != request.AcademyId)
            return Result<FacilityDto>.Failure("Facility does not belong to the specified academy.");

        if (facility.IsDeleted)
            return Result<FacilityDto>.Failure("Facility is deleted and cannot be updated.");

        if (request.FacilityName is not null)
            facility.FacilityName = request.FacilityName;

        if (request.FacilityType.HasValue)
            facility.FacilityType = request.FacilityType.Value;

        if (request.IndoorOutdoor is not null)
            facility.IndoorOutdoor = request.IndoorOutdoor;

        if (request.Capacity.HasValue)
            facility.Capacity = request.Capacity;

        if (request.Available.HasValue)
            facility.Available = request.Available.Value;

        if (request.Description is not null)
            facility.Description = request.Description;

        facility.UpdatedAt = DateTime.UtcNow;

        _academyFacilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility updated with Id: {FacilityId}", request.FacilityId);

        return Result<FacilityDto>.Success(new FacilityDto
        {
            Id = facility.Id,
            AcademyId = facility.AcademyId,
            FacilityName = facility.FacilityName,
            FacilityType = facility.FacilityType.ToString(),
            IndoorOutdoor = facility.IndoorOutdoor,
            Capacity = facility.Capacity,
            Available = facility.Available,
            Description = facility.Description,
            CreatedAt = facility.CreatedAt,
            UpdatedAt = facility.UpdatedAt
        });
    }
}
