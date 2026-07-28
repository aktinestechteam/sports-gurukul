using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class CertificateEngine : ICertificateEngine
{
    private readonly ILogger<CertificateEngine> _logger;

    public CertificateEngine(ILogger<CertificateEngine> logger)
    {
        _logger = logger;
    }

    public Task<string> GenerateCertificateNumberAsync(ProgramType programType, CancellationToken cancellationToken = default)
    {
        var prefix = programType switch
        {
            ProgramType.Event => "EVT",
            ProgramType.Training => "TRN",
            ProgramType.Workshop => "WRK",
            ProgramType.Camp => "CMP",
            ProgramType.Seminar => "SEM",
            ProgramType.Certification => "CRT",
            ProgramType.VirtualEvent => "VRT",
            _ => "PRG"
        };

        var code = $"{prefix}-CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        _logger.LogInformation("Generated certificate number: {CertificateNumber}", code);
        return Task.FromResult(code);
    }

    public Task<bool> IsEligibleForCertificateAsync(double attendanceRate, bool isProgramCompleted, double minimumAttendanceRate = 75.0)
    {
        var eligible = isProgramCompleted && attendanceRate >= minimumAttendanceRate;
        _logger.LogInformation("Certificate eligibility: {Eligible} (attendance: {AttendanceRate}%, completed: {IsCompleted}, threshold: {Threshold}%)", eligible, attendanceRate, isProgramCompleted, minimumAttendanceRate);
        return Task.FromResult(eligible);
    }

    public Task<CertificateType> DetermineCertificateTypeAsync(double attendanceRate, double? averageScore, CancellationToken cancellationToken = default)
    {
        CertificateType type;

        if (averageScore.HasValue && averageScore.Value >= 90)
        {
            type = CertificateType.Excellence;
        }
        else if (averageScore.HasValue && averageScore.Value >= 75)
        {
            type = CertificateType.Merit;
        }
        else if (attendanceRate >= 90)
        {
            type = CertificateType.Completion;
        }
        else
        {
            type = CertificateType.Participation;
        }

        _logger.LogInformation("Determined certificate type: {Type} (attendance: {AttendanceRate}%, score: {Score})", type, attendanceRate, averageScore);
        return Task.FromResult(type);
    }

    public Task<string?> SelectTemplateAsync(ProgramType programType, CertificateType certificateType, CancellationToken cancellationToken = default)
    {
        var templateId = $"template-{programType.ToString().ToLower()}-{certificateType.ToString().ToLower()}";
        _logger.LogInformation("Selected certificate template: {TemplateId}", templateId);
        return Task.FromResult<string?>(templateId);
    }
}
