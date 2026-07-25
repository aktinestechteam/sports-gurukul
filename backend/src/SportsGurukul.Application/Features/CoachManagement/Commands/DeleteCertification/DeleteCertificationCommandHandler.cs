using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;

public class DeleteCertificationCommandHandler : IRequestHandler<DeleteCertificationCommand, Result<Unit>>
{
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCertificationCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public DeleteCertificationCommandHandler(
        ICoachCertificationRepository coachCertificationRepository,
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCertificationCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachCertificationRepository = coachCertificationRepository;
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteCertificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting certification: {CertificationId}", request.CertificationId);

        var certification = await _coachCertificationRepository.GetByIdAsync(request.CertificationId, cancellationToken);
        if (certification is null || certification.IsDeleted)
        {
            _logger.LogWarning("Certification not found: {CertificationId}", request.CertificationId);
            return Result<Unit>.Failure("Certification not found.");
        }

        var coach = await _coachRepository.GetByIdAsync(certification.CoachId, cancellationToken);
        if (coach is not null && _currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        _coachCertificationRepository.Remove(certification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certification deleted: {CertificationId}", request.CertificationId);
        return Result<Unit>.Success(Unit.Value);
    }
}
