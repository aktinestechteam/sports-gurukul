using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachCertifications;

public class GetCoachCertificationsQueryHandler : IRequestHandler<GetCoachCertificationsQuery, Result<IReadOnlyList<CertificationDto>>>
{
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly ILogger<GetCoachCertificationsQueryHandler> _logger;

    public GetCoachCertificationsQueryHandler(
        ICoachCertificationRepository coachCertificationRepository,
        ILogger<GetCoachCertificationsQueryHandler> logger)
    {
        _coachCertificationRepository = coachCertificationRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CertificationDto>>> Handle(GetCoachCertificationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting certifications for coach Id: {CoachId}", request.CoachId);

        var certifications = await _coachCertificationRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);

        var dtos = certifications.Select(c => new CertificationDto
        {
            Id = c.Id,
            CertificationName = c.CertificationName,
            IssuingAuthority = c.IssuingAuthority,
            CertificateNumber = c.CertificateNumber,
            IssueDate = c.IssueDate,
            ExpiryDate = c.ExpiryDate,
            VerificationStatus = c.VerificationStatus.ToString(),
            CertificateUrl = c.CertificateUrl,
            IsExpired = c.ExpiryDate.HasValue && c.ExpiryDate < DateTime.UtcNow,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Result<IReadOnlyList<CertificationDto>>.Success(dtos);
    }
}
