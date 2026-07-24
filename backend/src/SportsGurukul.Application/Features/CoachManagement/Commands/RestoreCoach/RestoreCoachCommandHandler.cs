using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoach;

public class RestoreCoachCommandHandler : IRequestHandler<RestoreCoachCommand, Result<Unit>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreCoachCommandHandler> _logger;

    public RestoreCoachCommandHandler(
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreCoachCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RestoreCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring coach with Id: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId);
        if (coach is null)
            return Result<Unit>.Failure("Coach not found.");

        if (!coach.IsDeleted)
            return Result<Unit>.Failure("Coach is not deleted.");

        coach.IsDeleted = false;
        coach.Status = CoachStatus.Active;
        coach.UpdatedAt = DateTime.UtcNow;

        _coachRepository.Update(coach);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach restored with Id: {CoachId}", request.CoachId);

        return Result<Unit>.Success(Unit.Value);
    }
}
