using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreFacility;

public class RestoreFacilityCommandHandler : IRequestHandler<RestoreFacilityCommand, Result<FacilityDto>>
{
    private readonly IAcademyFacilityRepository _academyFacilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreFacilityCommandHandler> _logger;

    public RestoreFacilityCommandHandler(
        IAcademyFacilityRepository academyFacilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreFacilityCommandHandler> logger)
    {
        _academyFacilityRepository = academyFacilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDto>> Handle(RestoreFacilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring facility with Id: {FacilityId}", request.FacilityId);

        var facility = await _academyFacilityRepository.GetByIdAsync(request.FacilityId);
        if (facility is null)
            return Result<FacilityDto>.Failure("Facility not found.");

        if (!facility.IsDeleted)
            return Result<FacilityDto>.Failure("Facility is not deleted.");

        facility.IsDeleted = false;
        facility.UpdatedAt = DateTime.UtcNow;

        _academyFacilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility restored with Id: {FacilityId}", request.FacilityId);

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
