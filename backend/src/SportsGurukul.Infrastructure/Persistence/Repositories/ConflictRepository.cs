using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class ConflictRepository : Repository<BookingConflict>, IConflictRepository
{
    public ConflictRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BookingConflict>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingConflicts
            .AsNoTracking()
            .Where(bc => bc.BookingId == bookingId && !bc.IsDeleted)
            .OrderByDescending(bc => bc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingConflict>> GetUnresolvedByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.BookingConflicts
            .AsNoTracking()
            .Where(bc => bc.BookingId == bookingId
                && !bc.IsResolved
                && !bc.IsDeleted)
            .OrderByDescending(bc => bc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookingConflict>> GetByConflictTypeAsync(BookingConflictType conflictType, CancellationToken cancellationToken = default)
    {
        return await Context.BookingConflicts
            .AsNoTracking()
            .Where(bc => bc.ConflictType == conflictType && !bc.IsDeleted)
            .OrderByDescending(bc => bc.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
