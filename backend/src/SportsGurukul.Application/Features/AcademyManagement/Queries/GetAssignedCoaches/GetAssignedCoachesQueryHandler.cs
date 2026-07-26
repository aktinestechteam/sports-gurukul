using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAssignedCoaches;

public class GetAssignedCoachesQueryHandler : IRequestHandler<GetAssignedCoachesQuery, Result<IReadOnlyList<AcademyCoachSummaryDto>>>
{
    private readonly ICoachAcademyRepository _coachAcademyRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<GetAssignedCoachesQueryHandler> _logger;

    public GetAssignedCoachesQueryHandler(
        ICoachAcademyRepository coachAcademyRepository,
        ICoachRepository coachRepository,
        ILogger<GetAssignedCoachesQueryHandler> logger)
    {
        _coachAcademyRepository = coachAcademyRepository;
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AcademyCoachSummaryDto>>> Handle(GetAssignedCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assigned coaches for academy: {AcademyId}", request.AcademyId);

        var coachAcademies = await _coachAcademyRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);

        var dtos = new List<AcademyCoachSummaryDto>();
        foreach (var ca in coachAcademies)
        {
            var coach = await _coachRepository.GetByIdWithDetailsAsync(ca.CoachId, cancellationToken);
            if (coach is null) continue;

            dtos.Add(new AcademyCoachSummaryDto
            {
                Id = ca.Id,
                CoachId = coach.Id,
                CoachCode = coach.CoachCode,
                FullName = coach.User.FullName,
                Email = coach.User.Email,
                PhoneNumber = coach.User.PhoneNumber,
                ProfileImageUrl = coach.User.ProfileImageUrl,
                CoachingLevel = coach.CoachingLevel.ToString(),
                Status = coach.Status.ToString(),
                VerificationStatus = coach.VerificationStatus.ToString(),
                PrimarySport = coach.CoachSports?
                    .FirstOrDefault(s => s.IsPrimarySport)?.Sport?.Name,
                YearsOfExperience = coach.YearsOfExperience,
                AssignedDate = ca.AssignedDate
            });
        }

        _logger.LogInformation("Retrieved {Count} assigned coaches for academy: {AcademyId}", dtos.Count, request.AcademyId);

        return Result<IReadOnlyList<AcademyCoachSummaryDto>>.Success(dtos);
    }
}
