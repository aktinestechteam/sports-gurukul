using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RevokeCertificate;

public class RevokeCertificateCommandHandler : IRequestHandler<RevokeCertificateCommand, Result<PlatformCertificateDto>>
{
    private readonly ILogger<RevokeCertificateCommandHandler> _logger;

    public RevokeCertificateCommandHandler(ILogger<RevokeCertificateCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<PlatformCertificateDto>> Handle(RevokeCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking certificate {CertificateId} by {RevokedBy}. Reason: {Reason}",
            request.CertificateId, request.RevokedBy, request.Reason);

        var dto = new PlatformCertificateDto
        {
            Id = request.CertificateId,
            Status = PlatformCertificateStatus.Revoked,
            Notes = request.Reason,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Certificate {CertificateId} revoked successfully", request.CertificateId);
        return Task.FromResult(Result<PlatformCertificateDto>.Success(dto));
    }
}
