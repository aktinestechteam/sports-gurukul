using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class SessionRepository : Repository<TrainingSession>, ISessionRepository
{
    public SessionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TrainingSession?> GetBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionCode == sessionCode, cancellationToken);
    }

    public async Task<TrainingSession?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Batch)
            .Include(s => s.Coach)
            .Include(s => s.Facility)
            .Include(s => s.Attendances).ThenInclude(a => a.Athlete)
            .Include(s => s.Assessments).ThenInclude(a => a.Results)
            .Include(s => s.Schedules)
            .Include(s => s.Materials)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingSession>> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Coach)
            .Where(s => s.BatchId == batchId)
            .OrderBy(s => s.SessionDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingSession>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Batch).ThenInclude(b => b!.Program)
            .Where(s => s.CoachId == coachId)
            .OrderBy(s => s.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingSession>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Batch)
            .Where(s => s.FacilityId == facilityId)
            .OrderBy(s => s.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingSession>> GetBySessionDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Batch)
            .Include(s => s.Coach)
            .Where(s => s.SessionDate.Date == date.Date)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingSession>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Batch)
            .Include(s => s.Coach)
            .Where(s => s.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSessionCodeUniqueAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        return !await Context.TrainingSessions
            .AsNoTracking()
            .AnyAsync(s => s.SessionCode == sessionCode, cancellationToken);
    }
}
