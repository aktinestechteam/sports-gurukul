using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class WaitlistRepository : Repository<BookingWaitlist>, IWaitlistRepository
{
    public WaitlistRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BookingWaitlist>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingWaitlists
            .AsNoTracking()
            .Where(bw => bw.BookingId == bookingId && !bw.IsDeleted)
            .OrderBy(bw => bw.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingWaitlist>> GetActiveByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingWaitlists
            .AsNoTracking()
            .Where(bw => bw.BookingId == bookingId
                && bw.Status == WaitlistStatus.Active
                && !bw.IsDeleted)
            .OrderBy(bw => bw.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookingWaitlist?> GetByBookingAndUserAsync(Guid bookingId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingWaitlists
            .AsNoTracking()
            .FirstOrDefaultAsync(bw => bw.BookingId == bookingId
                && bw.WaitlistUserId == userId
                && !bw.IsDeleted, cancellationToken);
    }

    public async Task<int> GetMaxPriorityByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        var hasEntries = await Context.BookingWaitlists
            .AsNoTracking()
            .AnyAsync(bw => bw.BookingId == bookingId && !bw.IsDeleted, cancellationToken);

        if (!hasEntries)
            return 0;

        return await Context.BookingWaitlists
            .AsNoTracking()
            .Where(bw => bw.BookingId == bookingId && !bw.IsDeleted)
            .MaxAsync(bw => bw.Priority, cancellationToken);
    }
}
