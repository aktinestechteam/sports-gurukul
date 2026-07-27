using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class TrainingProgressRepository : Repository<TrainingProgress>, ITrainingProgressRepository
{
    public TrainingProgressRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TrainingProgress?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingProgresses
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId, cancellationToken);
    }

    public async Task<TrainingProgress?> GetByEnrollmentIdWithDetailsAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingProgresses
            .AsNoTracking()
            .Include(p => p.Enrollment).ThenInclude(e => e!.Batch).ThenInclude(b => b!.Program)
            .Include(p => p.Enrollment).ThenInclude(e => e!.Athlete)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingProgress>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingProgresses
            .AsNoTracking()
            .Include(p => p.Enrollment).ThenInclude(e => e!.Athlete)
            .Where(p => p.Enrollment != null && p.Enrollment.Batch != null && p.Enrollment.Batch.ProgramId == programId)
            .ToListAsync(cancellationToken);
    }
}
