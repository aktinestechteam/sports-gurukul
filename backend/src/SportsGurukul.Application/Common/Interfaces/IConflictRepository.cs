using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IConflictRepository : IRepository<BookingConflict>
{
    Task<IReadOnlyList<BookingConflict>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingConflict>> GetUnresolvedByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingConflict>> GetByConflictTypeAsync(BookingConflictType conflictType, CancellationToken cancellationToken = default);
}
