using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;

public class UpdateAchievementCommandHandler : IRequestHandler<UpdateAchievementCommand, Result<AthleteAchievementDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<Achievement> _achievementRepository;
    private readonly IRepository<AthleteAchievement> _athleteAchievementRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAchievementCommandHandler> _logger;

    public UpdateAchievementCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<Achievement> achievementRepository,
        IRepository<AthleteAchievement> athleteAchievementRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAchievementCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _achievementRepository = achievementRepository;
        _athleteAchievementRepository = athleteAchievementRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AthleteAchievementDto>> Handle(UpdateAchievementCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating achievement {AchievementId} for athlete {AthleteId}", request.AchievementId, request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<AthleteAchievementDto>.Failure("Athlete not found.");
        }

        var athleteAchievements = await _athleteAchievementRepository.FindAsync(
            aa => aa.AthleteId == request.AthleteId && aa.AchievementId == request.AchievementId, cancellationToken);

        var athleteAchievement = athleteAchievements.FirstOrDefault();
        if (athleteAchievement is null)
        {
            _logger.LogWarning("Achievement not found: {AthleteId}, {AchievementId}", request.AthleteId, request.AchievementId);
            return Result<AthleteAchievementDto>.Failure("Achievement not found for this athlete.");
        }

        var achievement = await _achievementRepository.GetByIdAsync(request.AchievementId, cancellationToken);
        if (achievement is null)
        {
            return Result<AthleteAchievementDto>.Failure("Achievement record not found.");
        }

        if (request.Title is not null) achievement.Title = request.Title;
        if (request.Competition is not null) achievement.Competition = request.Competition;
        if (request.Position is not null) achievement.Position = request.Position;
        if (request.Level.HasValue) achievement.Level = request.Level.Value;
        if (request.Date.HasValue) achievement.Date = request.Date.Value;
        if (request.CertificateUrl is not null) achievement.CertificateUrl = request.CertificateUrl;
        achievement.UpdatedAt = DateTime.UtcNow;
        _achievementRepository.Update(achievement);

        if (request.Notes is not null) athleteAchievement.Notes = request.Notes;
        athleteAchievement.UpdatedAt = DateTime.UtcNow;
        _athleteAchievementRepository.Update(athleteAchievement);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Achievement updated: {AchievementId}", request.AchievementId);

        var dto = new AthleteAchievementDto
        {
            Id = athleteAchievement.Id,
            AchievementId = achievement.Id,
            Title = achievement.Title,
            Competition = achievement.Competition,
            Position = achievement.Position,
            Level = achievement.Level.ToString(),
            Date = achievement.Date,
            CertificateUrl = achievement.CertificateUrl,
            AwardedDate = athleteAchievement.AwardedDate,
            Notes = athleteAchievement.Notes
        };

        return Result<AthleteAchievementDto>.Success(dto);
    }
}
