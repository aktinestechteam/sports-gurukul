using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;

public class AddAchievementCommandHandler : IRequestHandler<AddAchievementCommand, Result<AthleteAchievementDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<Achievement> _achievementRepository;
    private readonly IRepository<AthleteAchievement> _athleteAchievementRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddAchievementCommandHandler> _logger;

    public AddAchievementCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<Achievement> achievementRepository,
        IRepository<AthleteAchievement> athleteAchievementRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddAchievementCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _achievementRepository = achievementRepository;
        _athleteAchievementRepository = athleteAchievementRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AthleteAchievementDto>> Handle(AddAchievementCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding achievement to athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<AthleteAchievementDto>.Failure("Athlete not found.");
        }

        var achievement = new Achievement
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Competition = request.Competition,
            Position = request.Position,
            Level = request.Level,
            Date = request.Date,
            CertificateUrl = request.CertificateUrl
        };
        await _achievementRepository.AddAsync(achievement, cancellationToken);

        var athleteAchievement = new AthleteAchievement
        {
            Id = Guid.NewGuid(),
            AthleteId = request.AthleteId,
            AchievementId = achievement.Id,
            AwardedDate = DateTime.UtcNow,
            Notes = request.Notes
        };
        await _athleteAchievementRepository.AddAsync(athleteAchievement, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Achievement added: {AthleteId}, {Title}", request.AthleteId, request.Title);

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
