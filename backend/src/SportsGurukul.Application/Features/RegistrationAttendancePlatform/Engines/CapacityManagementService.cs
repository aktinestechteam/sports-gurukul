using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class CapacityManagementService : ICapacityManagementService
{
    private readonly ILogger<CapacityManagementService> _logger;

    public CapacityManagementService(ILogger<CapacityManagementService> logger)
    {
        _logger = logger;
    }

    public Task<bool> HasAvailableCapacityAsync(int currentCount, int? maxCapacity)
    {
        var hasCapacity = !maxCapacity.HasValue || currentCount < maxCapacity.Value;
        _logger.LogInformation("Capacity check: available={HasCapacity} (current={Current}, max={Max})", hasCapacity, currentCount, maxCapacity);
        return Task.FromResult(hasCapacity);
    }

    public Task<int> GetAvailableSlotsAsync(int currentCount, int? maxCapacity)
    {
        if (!maxCapacity.HasValue) return Task.FromResult(int.MaxValue);
        var available = Math.Max(0, maxCapacity.Value - currentCount);
        _logger.LogInformation("Available slots: {Available} (current={Current}, max={Max})", available, currentCount, maxCapacity);
        return Task.FromResult(available);
    }

    public Task<bool> IsAtCapacityAsync(int currentCount, int? maxCapacity)
    {
        if (!maxCapacity.HasValue) return Task.FromResult(false);
        var atCapacity = currentCount >= maxCapacity.Value;
        _logger.LogInformation("At capacity check: {AtCapacity} (current={Current}, max={Max})", atCapacity, currentCount, maxCapacity);
        return Task.FromResult(atCapacity);
    }

    public Task<int> CalculateNextWaitlistPositionAsync(int currentWaitlistCount)
    {
        var nextPosition = currentWaitlistCount + 1;
        _logger.LogInformation("Next waitlist position: {Position}", nextPosition);
        return Task.FromResult(nextPosition);
    }

    public Task<bool> ShouldAutoApproveAsync(ProgramType programType, EventRegistrationType registrationType, bool hasCapacity)
    {
        var autoApprove = hasCapacity && registrationType == EventRegistrationType.Free;
        _logger.LogInformation("Auto-approve check: {AutoApprove} (program={ProgramType}, regType={RegType}, hasCapacity={HasCapacity})", autoApprove, programType, registrationType, hasCapacity);
        return Task.FromResult(autoApprove);
    }
}
