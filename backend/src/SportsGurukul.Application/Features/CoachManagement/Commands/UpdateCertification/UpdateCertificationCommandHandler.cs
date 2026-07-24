using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCertification;

public class UpdateCertificationCommandHandler : IRequestHandler<UpdateCertificationCommand, Result<CertificationDto>>
{
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCertificationCommandHandler> _logger;

    public UpdateCertificationCommandHandler(
        ICoachCertificationRepository coachCertificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCertificationCommandHandler> logger)
    {
        _coachCertificationRepository = coachCertificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificationDto>> Handle(UpdateCertificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating certification: {CertificationId}", request.CertificationId);

        var certification = await _coachCertificationRepository.GetByIdAsync(request.CertificationId, cancellationToken);
        if (certification is null || certification.IsDeleted)
        {
            _logger.LogWarning("Certification not found: {CertificationId}", request.CertificationId);
            return Result<CertificationDto>.Failure("Certification not found.");
        }

        if (request.CertificationName is not null) certification.CertificationName = request.CertificationName;
        if (request.IssuingAuthority is not null) certification.IssuingAuthority = request.IssuingAuthority;
        if (request.CertificateNumber is not null) certification.CertificateNumber = request.CertificateNumber;
        if (request.IssueDate.HasValue) certification.IssueDate = request.IssueDate;
        if (request.ExpiryDate.HasValue) certification.ExpiryDate = request.ExpiryDate;
        if (request.CertificateUrl is not null) certification.CertificateUrl = request.CertificateUrl;
        certification.UpdatedAt = DateTime.UtcNow;

        _coachCertificationRepository.Update(certification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certification updated: {CertificationId}", request.CertificationId);

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
