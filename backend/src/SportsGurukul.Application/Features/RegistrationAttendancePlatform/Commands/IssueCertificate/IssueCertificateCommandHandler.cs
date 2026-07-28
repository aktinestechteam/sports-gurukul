using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.IssueCertificate;

public class IssueCertificateCommandHandler : IRequestHandler<IssueCertificateCommand, Result<PlatformCertificateDto>>
{
    private readonly ILogger<IssueCertificateCommandHandler> _logger;

    public IssueCertificateCommandHandler(ILogger<IssueCertificateCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<PlatformCertificateDto>> Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing certificate {CertificateId} by {IssuedBy}", request.CertificateId, request.IssuedBy);

        var dto = new PlatformCertificateDto
        {
            Id = request.CertificateId,
            Status = PlatformCertificateStatus.Issued,
            IssuedBy = request.IssuedBy,
            DocumentUrl = request.DocumentUrl,
            IsSent = true,
            IssuedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Certificate {CertificateId} issued successfully", request.CertificateId);
        return Task.FromResult(Result<PlatformCertificateDto>.Success(dto));
    }
}
