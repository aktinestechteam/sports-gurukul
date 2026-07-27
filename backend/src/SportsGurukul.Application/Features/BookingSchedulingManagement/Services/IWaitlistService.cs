using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface IWaitlistService
{
    Task<BookingWaitlist?> GetNextInWaitlistAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);
    Task<bool> PromoteWaitlistedBookingAsync(
        BookingWaitlist waitlistEntry,
        CancellationToken cancellationToken = default);
    Task<int> GetNextPriorityAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);
}
