using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Attendance>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Context.Attendances
            .AsNoTracking()
            .Include(a => a.Athlete)
            .Where(a => a.SessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Attendance>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Attendances
            .AsNoTracking()
            .Include(a => a.Session).ThenInclude(s => s!.Batch)
            .Where(a => a.AthleteId == athleteId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Attendance?> GetBySessionAndAthleteAsync(Guid sessionId, Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.AthleteId == athleteId, cancellationToken);
    }

    public async Task<IReadOnlyList<Attendance>> GetBySessionIdWithDetailsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Context.Attendances
            .AsNoTracking()
            .Include(a => a.Athlete).ThenInclude(ath => ath!.User)
            .Include(a => a.Session)
            .Where(a => a.SessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Attendance>> GetByStatusAsync(AttendanceStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Attendances
            .AsNoTracking()
            .Include(a => a.Session)
            .Include(a => a.Athlete)
            .Where(a => a.AttendanceStatus == status)
            .ToListAsync(cancellationToken);
    }
}
