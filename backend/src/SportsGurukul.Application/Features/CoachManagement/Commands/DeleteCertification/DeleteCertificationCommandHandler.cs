using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;

public class DeleteCertificationCommandHandler : IRequestHandler<DeleteCertificationCommand, Result<Unit>>
{
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCertificationCommandHandler> _logger;

    public DeleteCertificationCommandHandler(
        ICoachCertificationRepository coachCertificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCertificationCommandHandler> logger)
    {
        _coachCertificationRepository = coachCertificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
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

        _coachCertificationRepository.Remove(certification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certification deleted: {CertificationId}", request.CertificationId);
        return Result<Unit>.Success(Unit.Value);
    }
}
