using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveCoach;

public class RemoveCoachCommandHandler : IRequestHandler<RemoveCoachCommand, Result<Unit>>
{
    private readonly ICoachAcademyRepository _coachAcademyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveCoachCommandHandler> _logger;

    public RemoveCoachCommandHandler(
        ICoachAcademyRepository coachAcademyRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveCoachCommandHandler> logger)
    {
        _coachAcademyRepository = coachAcademyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RemoveCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing coach {CoachId} from academy {AcademyId}", request.CoachId, request.AcademyId);

        var coachAcademy = await _coachAcademyRepository.GetByAcademyAndCoachAsync(
            request.AcademyId, request.CoachId, cancellationToken);

        if (coachAcademy is null || !coachAcademy.IsActive)
            return Result<Unit>.Failure("Coach assignment not found.");

        coachAcademy.IsActive = false;
        coachAcademy.UpdatedAt = DateTime.UtcNow;

        _coachAcademyRepository.Update(coachAcademy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach {CoachId} removed from academy {AcademyId}", request.CoachId, request.AcademyId);

        return Result<Unit>.Success(Unit.Value);
    }
}
