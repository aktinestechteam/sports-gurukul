using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteAchievements;

public class GetAthleteAchievementsQueryHandler : IRequestHandler<GetAthleteAchievementsQuery, Result<IReadOnlyList<AthleteAchievementDto>>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteAchievementsQueryHandler> _logger;

    public GetAthleteAchievementsQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteAchievementsQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AthleteAchievementDto>>> Handle(GetAthleteAchievementsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching achievements for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<IReadOnlyList<AthleteAchievementDto>>.Failure("Athlete not found.");
        }

        var achievements = athlete.AthleteAchievements.Select(aa => new AthleteAchievementDto
        {
            Id = aa.Id,
            AchievementId = aa.AchievementId,
            Title = aa.Achievement.Title,
            Competition = aa.Achievement.Competition,
            Position = aa.Achievement.Position,
            Level = aa.Achievement.Level.ToString(),
            Date = aa.Achievement.Date,
            CertificateUrl = aa.Achievement.CertificateUrl,
            AwardedDate = aa.AwardedDate,
            Notes = aa.Notes
        }).ToList();

        return Result<IReadOnlyList<AthleteAchievementDto>>.Success(achievements);
    }
}
