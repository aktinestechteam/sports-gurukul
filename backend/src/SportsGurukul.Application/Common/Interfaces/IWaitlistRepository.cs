using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IWaitlistRepository : IRepository<BookingWaitlist>
{
    Task<IReadOnlyList<BookingWaitlist>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingWaitlist>> GetActiveByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<BookingWaitlist?> GetByBookingAndUserAsync(Guid bookingId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetMaxPriorityByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
}
