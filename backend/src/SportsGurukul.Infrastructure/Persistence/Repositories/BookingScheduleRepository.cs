using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class BookingScheduleRepository : Repository<BookingSchedule>, IBookingScheduleRepository
{
    public BookingScheduleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BookingSchedule>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingSchedules
            .AsNoTracking()
            .Where(bs => bs.BookingId == bookingId && !bs.IsDeleted)
            .OrderBy(bs => bs.ScheduledDate)
            .ThenBy(bs => bs.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingSchedule>> GetByDateAsync(Guid academyId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.BookingSchedules
            .AsNoTracking()
            .Include(bs => bs.Booking)
            .Where(bs => bs.Booking.AcademyId == academyId
                && bs.ScheduledDate.Date == date.Date
                && !bs.IsDeleted
                && !bs.Booking.IsDeleted)
            .OrderBy(bs => bs.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingSchedule>> GetByDateRangeAsync(Guid academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await Context.BookingSchedules
            .AsNoTracking()
            .Include(bs => bs.Booking)
            .Where(bs => bs.Booking.AcademyId == academyId
                && bs.ScheduledDate.Date >= startDate.Date
                && bs.ScheduledDate.Date <= endDate.Date
                && !bs.IsDeleted
                && !bs.Booking.IsDeleted)
            .OrderBy(bs => bs.ScheduledDate)
            .ThenBy(bs => bs.StartTime)
            .ToListAsync(cancellationToken);
    }
}
