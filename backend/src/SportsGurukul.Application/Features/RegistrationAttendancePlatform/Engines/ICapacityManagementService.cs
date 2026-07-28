using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface ICapacityManagementService
{
    Task<bool> HasAvailableCapacityAsync(int currentCount, int? maxCapacity);
    Task<int> GetAvailableSlotsAsync(int currentCount, int? maxCapacity);
    Task<bool> IsAtCapacityAsync(int currentCount, int? maxCapacity);
    Task<int> CalculateNextWaitlistPositionAsync(int currentWaitlistCount);
    Task<bool> ShouldAutoApproveAsync(ProgramType programType, EventRegistrationType registrationType, bool hasCapacity);
}
