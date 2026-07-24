using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;

public class VerifyCertificationCommandHandler : IRequestHandler<VerifyCertificationCommand, Result<CertificationDto>>
{
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyCertificationCommandHandler> _logger;

    public VerifyCertificationCommandHandler(
        ICoachCertificationRepository coachCertificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<VerifyCertificationCommandHandler> logger)
    {
        _coachCertificationRepository = coachCertificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificationDto>> Handle(VerifyCertificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating verification status for certification: {CertificationId} to {Status}", request.CertificationId, request.Status);

        var certification = await _coachCertificationRepository.GetByIdAsync(request.CertificationId, cancellationToken);
        if (certification is null || certification.IsDeleted)
        {
            _logger.LogWarning("Certification not found: {CertificationId}", request.CertificationId);
            return Result<CertificationDto>.Failure("Certification not found.");
        }

        certification.VerificationStatus = request.Status;
        certification.UpdatedAt = DateTime.UtcNow;

        _coachCertificationRepository.Update(certification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certification verification status updated: {CertificationId}", request.CertificationId);

        var dto = new CertificationDto
        {
            Id = certification.Id,
            CertificationName = certification.CertificationName,
            IssuingAuthority = certification.IssuingAuthority,
            CertificateNumber = certification.CertificateNumber,
            IssueDate = certification.IssueDate,
            ExpiryDate = certification.ExpiryDate,
            VerificationStatus = certification.VerificationStatus.ToString(),
            CertificateUrl = certification.CertificateUrl,
            IsExpired = certification.ExpiryDate.HasValue && certification.ExpiryDate.Value < DateTime.UtcNow,
            CreatedAt = certification.CreatedAt,
            UpdatedAt = certification.UpdatedAt
        };

        return Result<CertificationDto>.Success(dto);
    }
}
