using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;

public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, Result<FacilityDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyFacilityRepository _academyFacilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFacilityCommandHandler> _logger;

    public CreateFacilityCommandHandler(
        IAcademyRepository academyRepository,
        IAcademyFacilityRepository academyFacilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFacilityCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _academyFacilityRepository = academyFacilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FacilityDto>> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating facility '{FacilityName}' for AcademyId: {AcademyId}", request.FacilityName, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId);
        if (academy is null)
            return Result<FacilityDto>.Failure("Academy not found.");

        var facility = new AcademyFacility
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            FacilityName = request.FacilityName,
            FacilityType = request.FacilityType,
            IndoorOutdoor = request.IndoorOutdoor,
            Capacity = request.Capacity,
            Available = request.Available,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _academyFacilityRepository.AddAsync(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility created with Id: {FacilityId}, Name: {FacilityName}", facility.Id, facility.FacilityName);

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
