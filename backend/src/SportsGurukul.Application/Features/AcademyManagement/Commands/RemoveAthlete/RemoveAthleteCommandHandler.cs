using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveAthlete;

public class RemoveAthleteCommandHandler : IRequestHandler<RemoveAthleteCommand, Result<Unit>>
{
    private readonly IAthleteAcademyRepository _athleteAcademyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveAthleteCommandHandler> _logger;

    public RemoveAthleteCommandHandler(
        IAthleteAcademyRepository athleteAcademyRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveAthleteCommandHandler> logger)
    {
        _athleteAcademyRepository = athleteAcademyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RemoveAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing athlete {AthleteId} from academy {AcademyId}", request.AthleteId, request.AcademyId);

        var athleteAcademy = await _athleteAcademyRepository.GetByAcademyAndAthleteAsync(
            request.AcademyId, request.AthleteId, cancellationToken);

        if (athleteAcademy is null || !athleteAcademy.IsActive)
            return Result<Unit>.Failure("Athlete registration not found.");

        athleteAcademy.IsActive = false;
        athleteAcademy.UpdatedAt = DateTime.UtcNow;

        _athleteAcademyRepository.Update(athleteAcademy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete {AthleteId} removed from academy {AcademyId}", request.AthleteId, request.AcademyId);

        return Result<Unit>.Success(Unit.Value);
    }
}
