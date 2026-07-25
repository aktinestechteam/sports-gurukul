using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoach;

public class DeleteCoachCommandHandler : IRequestHandler<DeleteCoachCommand, Result<Unit>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCoachCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public DeleteCoachCommandHandler(
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCoachCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting coach with Id: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId);
        if (coach is null)
            return Result<Unit>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        if (coach.IsDeleted)
            return Result<Unit>.Failure("Coach is already deleted.");

        _coachRepository.Remove(coach);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach deleted with Id: {CoachId}", request.CoachId);

        return Result<Unit>.Success(Unit.Value);
    }
}
