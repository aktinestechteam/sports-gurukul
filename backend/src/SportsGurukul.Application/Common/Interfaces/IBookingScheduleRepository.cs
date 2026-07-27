using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IBookingScheduleRepository : IRepository<BookingSchedule>
{
    Task<IReadOnlyList<BookingSchedule>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingSchedule>> GetByDateAsync(Guid academyId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingSchedule>> GetByDateRangeAsync(Guid academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
