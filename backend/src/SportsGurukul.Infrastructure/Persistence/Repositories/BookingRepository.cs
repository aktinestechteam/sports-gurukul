using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Booking?> GetByBookingNumberAsync(string bookingNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber, cancellationToken);
    }

    public async Task<Booking?> GetWithDetailsAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Include(b => b.Academy)
            .Include(b => b.Branch)
            .Include(b => b.Facility)
            .Include(b => b.Coach)
            .Include(b => b.Athlete)
            .Include(b => b.TrainingSession)
            .Include(b => b.Items)
            .Include(b => b.Participants)
            .Include(b => b.Schedules)
            .Include(b => b.Recurrences)
            .Include(b => b.WaitlistEntries)
            .Include(b => b.Cancellations)
            .Include(b => b.Reschedules)
            .Include(b => b.Reminders)
            .Include(b => b.Approvals)
            .Include(b => b.Conflicts)
            .Include(b => b.History)
            .Include(b => b.Attachments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Where(b => b.AcademyId == academyId && !b.IsDeleted)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByFacilityIdAsync(Guid facilityId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Where(b => b.FacilityId == facilityId
                && b.BookingDate.Date == date.Date
                && !b.IsDeleted)
            .OrderBy(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByCoachIdAsync(Guid coachId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Where(b => b.CoachId == coachId
                && b.BookingDate.Date == date.Date
                && !b.IsDeleted)
            .OrderBy(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Where(b => b.AthleteId == athleteId && !b.IsDeleted)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByDateRangeAsync(Guid academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await Context.Bookings
            .AsNoTracking()
            .Where(b => b.AcademyId == academyId
                && b.BookingDate.Date >= startDate.Date
                && b.BookingDate.Date <= endDate.Date
                && !b.IsDeleted)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> SearchAsync(
        Guid? academyId,
        Guid? branchId,
        BookingType? bookingType,
        BookingStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Bookings
            .AsNoTracking()
            .Where(b => !b.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(b => b.AcademyId == academyId.Value);

        if (branchId.HasValue)
            query = query.Where(b => b.BranchId == branchId.Value);

        if (bookingType.HasValue)
            query = query.Where(b => b.BookingType == bookingType.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b =>
                b.BookingNumber.Contains(searchTerm) ||
                b.Title.Contains(searchTerm) ||
                (b.Description != null && b.Description.Contains(searchTerm)));

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? academyId,
        Guid? branchId,
        BookingType? bookingType,
        BookingStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Bookings
            .AsNoTracking()
            .Where(b => !b.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(b => b.AcademyId == academyId.Value);

        if (branchId.HasValue)
            query = query.Where(b => b.BranchId == branchId.Value);

        if (bookingType.HasValue)
            query = query.Where(b => b.BookingType == bookingType.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b =>
                b.BookingNumber.Contains(searchTerm) ||
                b.Title.Contains(searchTerm) ||
                (b.Description != null && b.Description.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsBookingNumberUniqueAsync(string bookingNumber, CancellationToken cancellationToken = default)
    {
        return !await Context.Bookings
            .AsNoTracking()
            .AnyAsync(b => b.BookingNumber == bookingNumber && !b.IsDeleted, cancellationToken);
    }
}
