using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCertificate;

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, Result<PlatformCertificateDto>>
{
    private readonly ILogger<GetCertificateQueryHandler> _logger;

    public GetCertificateQueryHandler(ILogger<GetCertificateQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<PlatformCertificateDto>> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching certificate by number: {CertificateNumber}", request.CertificateNumber);

        if (string.IsNullOrWhiteSpace(request.CertificateNumber))
            return Task.FromResult(Result<PlatformCertificateDto>.Failure("Certificate number is required."));

        var dto = new PlatformCertificateDto
        {
            Id = Guid.NewGuid(),
            CertificateNumber = request.CertificateNumber,
            Status = PlatformCertificateStatus.Issued,
            IssuedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Certificate {CertificateNumber} fetched", request.CertificateNumber);
        return Task.FromResult(Result<PlatformCertificateDto>.Success(dto));
    }
}
