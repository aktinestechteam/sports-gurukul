using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;

public class RestoreAthleteCommandHandler : IRequestHandler<RestoreAthleteCommand, Result<Unit>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreAthleteCommandHandler> _logger;

    public RestoreAthleteCommandHandler(
        IAthleteRepository athleteRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreAthleteCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RestoreAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is not null && !athlete.IsDeleted)
        {
            _logger.LogWarning("Athlete is not deleted: {AthleteId}", request.AthleteId);
            return Result<Unit>.Failure("Athlete is not deleted.");
        }

        var deletedAthlete = await _athleteRepository.GetDeletedByUserIdAsync(request.AthleteId, cancellationToken);
        if (deletedAthlete is null)
        {
            _logger.LogWarning("Deleted athlete not found: {AthleteId}", request.AthleteId);
            return Result<Unit>.Failure("Deleted athlete not found.");
        }

        deletedAthlete.IsDeleted = false;
        deletedAthlete.Status = AthleteStatus.Active;
        deletedAthlete.UpdatedAt = DateTime.UtcNow;
        _athleteRepository.Update(deletedAthlete);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete restored: {AthleteId}", request.AthleteId);
        return Result<Unit>.Success(Unit.Value);
    }
}
