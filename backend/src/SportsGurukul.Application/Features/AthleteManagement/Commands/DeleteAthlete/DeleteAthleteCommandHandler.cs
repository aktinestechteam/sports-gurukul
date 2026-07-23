using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;

public class DeleteAthleteCommandHandler : IRequestHandler<DeleteAthleteCommand, Result<Unit>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAthleteCommandHandler> _logger;

    public DeleteAthleteCommandHandler(
        IAthleteRepository athleteRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAthleteCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<Unit>.Failure("Athlete not found.");
        }

        if (athlete.IsDeleted)
        {
            _logger.LogWarning("Athlete already deleted: {AthleteId}", request.AthleteId);
            return Result<Unit>.Failure("Athlete is already deleted.");
        }

        _athleteRepository.Remove(athlete);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete deleted: {AthleteId}, Code: {AthleteCode}", athlete.Id, athlete.AthleteCode);
        return Result<Unit>.Success(Unit.Value);
    }
}
