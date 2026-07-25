using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetAssignedAthletes;

public class GetAssignedAthletesQueryHandler : IRequestHandler<GetAssignedAthletesQuery, Result<IReadOnlyList<AssignedAthleteDto>>>
{
    private readonly IRepository<CoachAthlete> _coachAthleteRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAssignedAthletesQueryHandler> _logger;

    public GetAssignedAthletesQueryHandler(
        IRepository<CoachAthlete> coachAthleteRepository,
        IAthleteRepository athleteRepository,
        ILogger<GetAssignedAthletesQueryHandler> logger)
    {
        _coachAthleteRepository = coachAthleteRepository;
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AssignedAthleteDto>>> Handle(GetAssignedAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assigned athletes for coach: {CoachId}", request.CoachId);

        var coachAthletes = await _coachAthleteRepository.FindAsync(
            x => x.CoachId == request.CoachId && x.IsActive && !x.IsDeleted,
            cancellationToken);

        var dtos = new List<AssignedAthleteDto>();
        foreach (var ca in coachAthletes)
        {
            var athlete = await _athleteRepository.GetByIdWithDetailsAsync(ca.AthleteId, cancellationToken);
            if (athlete is null) continue;

            dtos.Add(new AssignedAthleteDto
            {
                Id = ca.Id,
                AthleteId = athlete.Id,
                AthleteCode = athlete.AthleteCode,
                FullName = athlete.User.FullName,
                Email = athlete.User.Email,
                PhoneNumber = athlete.User.PhoneNumber,
                ProfileImageUrl = athlete.User.ProfileImageUrl,
                CurrentLevel = athlete.CurrentLevel.ToString(),
                Status = athlete.Status.ToString(),
                PrimarySport = athlete.AthleteSports?
                    .FirstOrDefault(s => s.IsPrimarySport)?.Sport?.Name,
                AssignedDate = ca.AssignedDate
            });
        }

        _logger.LogInformation("Retrieved {Count} assigned athletes for coach: {CoachId}", dtos.Count, request.CoachId);

        return Result<IReadOnlyList<AssignedAthleteDto>>.Success(dtos);
    }
}
