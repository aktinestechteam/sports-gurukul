using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class WaitlistEngine : IWaitlistEngine
{
    private readonly ILogger<WaitlistEngine> _logger;

    public WaitlistEngine(ILogger<WaitlistEngine> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanPromoteAsync(WaitlistStatus currentStatus, bool hasCapacityAvailable)
    {
        var canPromote = currentStatus == WaitlistStatus.Active && hasCapacityAvailable;
        if (!canPromote)
        {
            _logger.LogWarning("Waitlist promotion not possible: status={Status}, hasCapacity={HasCapacity}", currentStatus, hasCapacityAvailable);
        }
        return Task.FromResult(canPromote);
    }

    public Task<WaitlistStatus> DetermineWaitlistStatusAsync(bool hasCapacity, bool waitlistEnabled)
    {
        WaitlistStatus status;

        if (hasCapacity)
        {
            status = WaitlistStatus.Promoted;
        }
        else if (waitlistEnabled)
        {
            status = WaitlistStatus.Active;
        }
        else
        {
            status = WaitlistStatus.Expired;
        }

        _logger.LogInformation("Determined waitlist status: {Status} (hasCapacity={HasCapacity}, waitlistEnabled={WaitlistEnabled})", status, hasCapacity, waitlistEnabled);
        return Task.FromResult(status);
    }

    public Task<DateTime?> CalculateExpirationAsync(ProgramType programType, CancellationToken cancellationToken = default)
    {
        var days = programType switch
        {
            ProgramType.Event => 7,
            ProgramType.Training => 14,
            ProgramType.Workshop => 3,
            ProgramType.Camp => 7,
            ProgramType.Seminar => 3,
            ProgramType.Certification => 14,
            ProgramType.VirtualEvent => 3,
            _ => 7
        };

        var expiration = DateTime.UtcNow.AddDays(days);
        _logger.LogInformation("Waitlist expiration for {ProgramType}: {Expiration}", programType, expiration);
        return Task.FromResult<DateTime?>(expiration);
    }

    public Task<int> GetPromotionOrderAsync(int currentPosition)
    {
        return Task.FromResult(currentPosition);
    }
}
