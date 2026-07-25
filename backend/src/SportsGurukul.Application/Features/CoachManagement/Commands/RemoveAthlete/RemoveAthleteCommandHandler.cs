using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RemoveAthlete;

public class RemoveAthleteCommandHandler : IRequestHandler<RemoveAthleteCommand, Result<Unit>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IRepository<CoachAthlete> _coachAthleteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveAthleteCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public RemoveAthleteCommandHandler(
        ICoachRepository coachRepository,
        IRepository<CoachAthlete> coachAthleteRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveAthleteCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _coachAthleteRepository = coachAthleteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(RemoveAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing athlete {AthleteId} from coach {CoachId}", request.AthleteId, request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<Unit>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        var assignments = await _coachAthleteRepository.FindAsync(
            ca => ca.CoachId == request.CoachId && ca.AthleteId == request.AthleteId && ca.IsActive, cancellationToken);

        var coachAthlete = assignments.FirstOrDefault();
        if (coachAthlete is null)
            return Result<Unit>.Failure("Athlete assignment not found.");

        coachAthlete.IsActive = false;
        coachAthlete.UpdatedAt = DateTime.UtcNow;

        _coachAthleteRepository.Update(coachAthlete);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete {AthleteId} removed from coach {CoachId}", request.AthleteId, request.CoachId);

        return Result<Unit>.Success(Unit.Value);
    }
}
