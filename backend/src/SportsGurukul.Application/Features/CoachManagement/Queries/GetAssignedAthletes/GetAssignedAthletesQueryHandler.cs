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
    private readonly ILogger<GetAssignedAthletesQueryHandler> _logger;

    public GetAssignedAthletesQueryHandler(
        IRepository<CoachAthlete> coachAthleteRepository,
        ILogger<GetAssignedAthletesQueryHandler> logger)
    {
        _coachAthleteRepository = coachAthleteRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AssignedAthleteDto>>> Handle(GetAssignedAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assigned athletes for coach: {CoachId}", request.CoachId);

        var coachAthletes = await _coachAthleteRepository.FindAsync(
            x => x.CoachId == request.CoachId && x.IsActive && !x.IsDeleted,
            cancellationToken);

        var dtos = coachAthletes.Select(ca => new AssignedAthleteDto
        {
            Id = ca.Id,
            AthleteId = ca.AthleteId,
            AthleteCode = ca.Athlete.AthleteCode,
            FullName = ca.Athlete.User.FullName,
            Email = ca.Athlete.User.Email,
            PhoneNumber = ca.Athlete.User.PhoneNumber,
            ProfileImageUrl = ca.Athlete.User.ProfileImageUrl,
            CurrentLevel = ca.Athlete.CurrentLevel.ToString(),
            Status = ca.Athlete.Status.ToString(),
            PrimarySport = ca.Athlete.AthleteSports?
                .FirstOrDefault(s => s.IsPrimarySport)?.Sport?.Name,
            AssignedDate = ca.AssignedDate
        }).ToList();

        _logger.LogInformation("Retrieved {Count} assigned athletes for coach: {CoachId}", dtos.Count, request.CoachId);

        return Result<IReadOnlyList<AssignedAthleteDto>>.Success(dtos);
    }
}
