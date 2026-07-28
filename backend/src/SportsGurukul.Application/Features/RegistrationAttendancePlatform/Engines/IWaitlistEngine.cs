using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface IWaitlistEngine
{
    Task<bool> CanPromoteAsync(WaitlistStatus currentStatus, bool hasCapacityAvailable);
    Task<WaitlistStatus> DetermineWaitlistStatusAsync(bool hasCapacity, bool waitlistEnabled);
    Task<DateTime?> CalculateExpirationAsync(ProgramType programType, CancellationToken cancellationToken = default);
    Task<int> GetPromotionOrderAsync(int currentPosition);
}
