using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;

public class AddCertificationCommandHandler : IRequestHandler<AddCertificationCommand, Result<CertificationDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCertificationCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public AddCertificationCommandHandler(
        ICoachRepository coachRepository,
        ICoachCertificationRepository coachCertificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCertificationCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _coachCertificationRepository = coachCertificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<CertificationDto>> Handle(AddCertificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding certification to coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning("Coach not found: {CoachId}", request.CoachId);
            return Result<CertificationDto>.Failure("Coach not found.");
        }

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<CertificationDto>.Failure("You are not authorized to modify this coach's data.");

        var existingCertifications = await _coachCertificationRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);
        if (existingCertifications.Any(c => c.CertificationName == request.CertificationName && !c.IsDeleted))
        {
            _logger.LogWarning("Certification already exists: {CertificationName}, {CoachId}", request.CertificationName, request.CoachId);
            return Result<CertificationDto>.Failure("A certification with this name already exists for this coach.");
        }

        var certification = new CoachCertification
        {
            Id = Guid.NewGuid(),
            CoachId = request.CoachId,
            CertificationName = request.CertificationName,
            IssuingAuthority = request.IssuingAuthority,
            CertificateNumber = request.CertificateNumber,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            CertificateUrl = request.CertificateUrl,
            VerificationStatus = VerificationStatus.Pending
        };

        await _coachCertificationRepository.AddAsync(certification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certification added: {CertificationId}, {CoachId}", certification.Id, request.CoachId);

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
