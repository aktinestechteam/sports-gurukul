using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetFacilities;

public class GetFacilitiesQueryHandler : IRequestHandler<GetFacilitiesQuery, Result<IReadOnlyList<FacilityDto>>>
{
    private readonly IAcademyFacilityRepository _facilityRepository;
    private readonly ILogger<GetFacilitiesQueryHandler> _logger;

    public GetFacilitiesQueryHandler(
        IAcademyFacilityRepository facilityRepository,
        ILogger<GetFacilitiesQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<FacilityDto>>> Handle(GetFacilitiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching facilities for academy: {AcademyId}", request.AcademyId);

        var facilities = await _facilityRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);

        var dtos = facilities.Select(f => new FacilityDto
        {
            Id = f.Id,
            AcademyId = f.AcademyId,
            FacilityName = f.FacilityName,
            FacilityType = f.FacilityType.ToString(),
            IndoorOutdoor = f.IndoorOutdoor,
            Capacity = f.Capacity,
            Available = f.Available,
            Description = f.Description,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} facilities for academy: {AcademyId}", dtos.Count, request.AcademyId);

        return Result<IReadOnlyList<FacilityDto>>.Success(dtos);
    }
}
