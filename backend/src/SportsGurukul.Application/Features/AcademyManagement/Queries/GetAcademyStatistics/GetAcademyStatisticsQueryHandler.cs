using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyStatistics;

public class GetAcademyStatisticsQueryHandler : IRequestHandler<GetAcademyStatisticsQuery, Result<AcademyStatisticsDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyBranchRepository _branchRepository;
    private readonly IAcademyFacilityRepository _facilityRepository;
    private readonly IAcademyMembershipRepository _membershipRepository;
    private readonly ICoachAcademyRepository _coachAcademyRepository;
    private readonly IAthleteAcademyRepository _athleteAcademyRepository;
    private readonly ILogger<GetAcademyStatisticsQueryHandler> _logger;

    public GetAcademyStatisticsQueryHandler(
        IAcademyRepository academyRepository,
        IAcademyBranchRepository branchRepository,
        IAcademyFacilityRepository facilityRepository,
        IAcademyMembershipRepository membershipRepository,
        ICoachAcademyRepository coachAcademyRepository,
        IAthleteAcademyRepository athleteAcademyRepository,
        ILogger<GetAcademyStatisticsQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _branchRepository = branchRepository;
        _facilityRepository = facilityRepository;
        _membershipRepository = membershipRepository;
        _coachAcademyRepository = coachAcademyRepository;
        _athleteAcademyRepository = athleteAcademyRepository;
        _logger = logger;
    }

    public async Task<Result<AcademyStatisticsDto>> Handle(GetAcademyStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting statistics for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<AcademyStatisticsDto>.Failure("Academy not found.");

        var branches = await _branchRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        var facilities = await _facilityRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        var memberships = await _membershipRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        var coachAcademies = await _coachAcademyRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        var athleteAcademies = await _athleteAcademyRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        var documents = await _academyRepository.GetDocumentsAsync(request.AcademyId, cancellationToken);
        var galleryImages = await _academyRepository.GetGalleryImagesAsync(request.AcademyId, cancellationToken);

        var statistics = new AcademyStatisticsDto
        {
            AcademyId = academy.Id,
            AcademyName = academy.Name,
            TotalCoaches = coachAcademies.Count(ca => ca.IsActive),
            TotalAthletes = athleteAcademies.Count(aa => aa.IsActive),
            TotalBranches = branches.Count,
            TotalFacilities = facilities.Count,
            ActiveMemberships = memberships.Count,
            SportsOffered = academy.AcademySports?.Count ?? 0,
            TotalDocuments = documents.Count,
            TotalGalleryImages = galleryImages.Count
        };

        return Result<AcademyStatisticsDto>.Success(statistics);
    }
}
