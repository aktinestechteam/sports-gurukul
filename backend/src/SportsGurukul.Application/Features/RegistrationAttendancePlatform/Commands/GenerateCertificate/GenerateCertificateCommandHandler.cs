using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateCertificate;

public class GenerateCertificateCommandHandler : IRequestHandler<GenerateCertificateCommand, Result<PlatformCertificateDto>>
{
    private readonly ICertificateEngine _certificateEngine;
    private readonly ILogger<GenerateCertificateCommandHandler> _logger;

    public GenerateCertificateCommandHandler(
        ICertificateEngine certificateEngine,
        ILogger<GenerateCertificateCommandHandler> logger)
    {
        _certificateEngine = certificateEngine;
        _logger = logger;
    }

    public async Task<Result<PlatformCertificateDto>> Handle(GenerateCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating certificate for participant {ParticipantId} on {ProgramType} {ProgramId}",
            request.ParticipantId, request.ProgramType, request.ProgramId);

        var isEligible = await _certificateEngine.IsEligibleForCertificateAsync(
            request.AttendanceRate, true, 0.75);
        if (!isEligible)
            return Result<PlatformCertificateDto>.Failure("Participant is not eligible for certificate generation.");

        var certificateNumber = await _certificateEngine.GenerateCertificateNumberAsync(request.ProgramType, cancellationToken);
        var certificateType = await _certificateEngine.DetermineCertificateTypeAsync(
            request.AttendanceRate, request.AverageScore, cancellationToken);
        var templateId = await _certificateEngine.SelectTemplateAsync(request.ProgramType, certificateType, cancellationToken);

        var dto = new PlatformCertificateDto
        {
            Id = Guid.NewGuid(),
            ProgramType = request.ProgramType,
            ProgramId = request.ProgramId,
            ParticipantId = request.ParticipantId,
            CertificateNumber = certificateNumber,
            CertificateType = certificateType,
            Status = PlatformCertificateStatus.Generated,
            IssuedDate = DateTime.UtcNow,
            IssuedBy = request.IssuedBy,
            TemplateId = templateId,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Certificate generated: {CertificateNumber}, type: {CertificateType}", certificateNumber, certificateType);
        return Result<PlatformCertificateDto>.Success(dto);
    }
}
