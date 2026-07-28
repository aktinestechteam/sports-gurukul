using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface IRegistrationEngine
{
    Task<string> GenerateRegistrationNumberAsync(ProgramType programType, CancellationToken cancellationToken = default);
    Task<PlatformRegistrationStatus> DetermineInitialStatusAsync(ProgramType programType, EventRegistrationType registrationType, CancellationToken cancellationToken = default);
    Task<bool> ValidateRegistrationEligibilityAsync(ProgramType programType, Guid programId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default);
    Task<bool> IsDuplicateRegistrationAsync(ProgramType programType, Guid programId, Guid? athleteId, Guid? userId, Func<ProgramType, Guid, Guid?, Guid?, CancellationToken, Task<bool>> duplicateCheck, CancellationToken cancellationToken = default);
}
