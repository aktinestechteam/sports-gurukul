using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class WaitlistService : IWaitlistService
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly ILogger<WaitlistService> _logger;

    public WaitlistService(
        IWaitlistRepository waitlistRepository,
        ILogger<WaitlistService> logger)
    {
        _waitlistRepository = waitlistRepository;
        _logger = logger;
    }

    public async Task<BookingWaitlist?> GetNextInWaitlistAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var activeEntries = await _waitlistRepository
            .GetActiveByBookingIdAsync(bookingId, cancellationToken);

        return activeEntries
            .OrderBy(w => w.Priority)
            .ThenBy(w => w.RequestedOn)
            .FirstOrDefault();
    }

    public async Task<bool> PromoteWaitlistedBookingAsync(
        BookingWaitlist waitlistEntry,
        CancellationToken cancellationToken = default)
    {
        if (waitlistEntry.Status != WaitlistStatus.Active)
        {
            _logger.LogWarning(
                "Cannot promote waitlist entry {Id} with status {Status}",
                waitlistEntry.Id, waitlistEntry.Status);
            return false;
        }

        waitlistEntry.Status = WaitlistStatus.Promoted;
        waitlistEntry.PromotionOrder = waitlistEntry.Priority;

        _logger.LogInformation(
            "Promoted waitlist entry {Id} for booking {BookingId}",
            waitlistEntry.Id, waitlistEntry.BookingId);

        return true;
    }

    public async Task<int> GetNextPriorityAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        return await _waitlistRepository
            .GetMaxPriorityByBookingIdAsync(bookingId, cancellationToken) + 1;
    }
}
