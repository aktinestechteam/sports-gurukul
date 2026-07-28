using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class RegistrationEngine : IRegistrationEngine
{
    private readonly ILogger<RegistrationEngine> _logger;

    public RegistrationEngine(ILogger<RegistrationEngine> logger)
    {
        _logger = logger;
    }

    public Task<string> GenerateRegistrationNumberAsync(ProgramType programType, CancellationToken cancellationToken = default)
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

        var code = $"{prefix}-REG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        _logger.LogInformation("Generated registration number: {RegistrationNumber}", code);
        return Task.FromResult(code);
    }

    public Task<PlatformRegistrationStatus> DetermineInitialStatusAsync(ProgramType programType, EventRegistrationType registrationType, CancellationToken cancellationToken = default)
    {
        var status = registrationType switch
        {
            EventRegistrationType.Free => PlatformRegistrationStatus.Approved,
            EventRegistrationType.Paid => PlatformRegistrationStatus.Pending,
            EventRegistrationType.Invitation => PlatformRegistrationStatus.Pending,
            EventRegistrationType.ApprovalRequired => PlatformRegistrationStatus.Pending,
            EventRegistrationType.Waitlist => PlatformRegistrationStatus.Waitlisted,
            _ => PlatformRegistrationStatus.Pending
        };

        _logger.LogInformation("Determined initial status {Status} for program type {ProgramType} with registration type {RegistrationType}", status, programType, registrationType);
        return Task.FromResult(status);
    }

    public Task<bool> ValidateRegistrationEligibilityAsync(ProgramType programType, Guid programId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default)
    {
        if (athleteId == null && userId == null)
        {
            _logger.LogWarning("Registration rejected: no participant identified for program {ProgramId}", programId);
            return Task.FromResult(false);
        }

        _logger.LogInformation("Registration eligibility validated for program {ProgramId}, program type {ProgramType}", programId, programType);
        return Task.FromResult(true);
    }

    public async Task<bool> IsDuplicateRegistrationAsync(ProgramType programType, Guid programId, Guid? athleteId, Guid? userId, Func<ProgramType, Guid, Guid?, Guid?, CancellationToken, Task<bool>> duplicateCheck, CancellationToken cancellationToken = default)
    {
        var isDuplicate = await duplicateCheck(programType, programId, athleteId, userId, cancellationToken);
        if (isDuplicate)
        {
            _logger.LogWarning("Duplicate registration detected for program {ProgramId}, athlete {AthleteId}, user {UserId}", programId, athleteId, userId);
        }
        return isDuplicate;
    }
}
