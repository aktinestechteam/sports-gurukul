using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;

public class RemoveSportCommandHandler : IRequestHandler<RemoveSportCommand, Result<Unit>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IRepository<CoachSport> _coachSportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveSportCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public RemoveSportCommandHandler(
        ICoachRepository coachRepository,
        IRepository<CoachSport> coachSportRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveSportCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _coachSportRepository = coachSportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(RemoveSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing sport {SportId} from coach {CoachId}", request.SportId, request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<Unit>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        var coachSports = await _coachRepository.GetCoachSportsAsync(request.CoachId, cancellationToken);
        var coachSport = coachSports.FirstOrDefault(s => s.SportId == request.SportId && !s.IsDeleted);

        if (coachSport is null)
        {
            _logger.LogWarning("Sport not assigned: {SportId}, {CoachId}", request.SportId, request.CoachId);
            return Result<Unit>.Failure("This sport is not assigned to the coach.");
        }

        _coachSportRepository.Remove(coachSport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport removed: {SportId}, {CoachId}", request.SportId, request.CoachId);
        return Result<Unit>.Success(Unit.Value);
    }
}
