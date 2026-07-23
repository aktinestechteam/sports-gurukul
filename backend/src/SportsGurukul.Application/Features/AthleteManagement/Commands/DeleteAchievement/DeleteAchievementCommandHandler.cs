using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;

public class DeleteAchievementCommandHandler : IRequestHandler<DeleteAchievementCommand, Result<Unit>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<AthleteAchievement> _athleteAchievementRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAchievementCommandHandler> _logger;

    public DeleteAchievementCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<AthleteAchievement> athleteAchievementRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAchievementCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _athleteAchievementRepository = athleteAchievementRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteAchievementCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting achievement {AchievementId} from athlete {AthleteId}", request.AchievementId, request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<Unit>.Failure("Athlete not found.");
        }

        var athleteAchievements = await _athleteAchievementRepository.FindAsync(
            aa => aa.AthleteId == request.AthleteId && aa.AchievementId == request.AchievementId, cancellationToken);

        var athleteAchievement = athleteAchievements.FirstOrDefault();
        if (athleteAchievement is null)
        {
            _logger.LogWarning("Achievement not found: {AthleteId}, {AchievementId}", request.AthleteId, request.AchievementId);
            return Result<Unit>.Failure("Achievement not found for this athlete.");
        }

        _athleteAchievementRepository.Remove(athleteAchievement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Achievement deleted: {AthleteId}, {AchievementId}", request.AthleteId, request.AchievementId);
        return Result<Unit>.Success(Unit.Value);
    }
}
