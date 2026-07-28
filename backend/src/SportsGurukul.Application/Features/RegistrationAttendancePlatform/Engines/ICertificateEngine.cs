using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface ICertificateEngine
{
    Task<string> GenerateCertificateNumberAsync(ProgramType programType, CancellationToken cancellationToken = default);
    Task<bool> IsEligibleForCertificateAsync(double attendanceRate, bool isProgramCompleted, double minimumAttendanceRate = 75.0);
    Task<CertificateType> DetermineCertificateTypeAsync(double attendanceRate, double? averageScore, CancellationToken cancellationToken = default);
    Task<string?> SelectTemplateAsync(ProgramType programType, CertificateType certificateType, CancellationToken cancellationToken = default);
}
