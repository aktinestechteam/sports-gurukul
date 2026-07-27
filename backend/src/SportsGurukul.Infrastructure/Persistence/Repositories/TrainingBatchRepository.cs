using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class TrainingBatchRepository : Repository<TrainingBatch>, ITrainingBatchRepository
{
    public TrainingBatchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TrainingBatch?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BatchCode == batchCode, cancellationToken);
    }

    public async Task<TrainingBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .Include(b => b.Program)
            .Include(b => b.Coach)
            .Include(b => b.Branch)
            .Include(b => b.Sessions)
            .Include(b => b.Enrollments).ThenInclude(e => e.Athlete)
            .AsSplitQuery()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingBatch>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .Include(b => b.Coach)
            .Where(b => b.ProgramId == programId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingBatch>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .Include(b => b.Program)
            .Where(b => b.CoachId == coachId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingBatch>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .Include(b => b.Program)
            .Where(b => b.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingBatch>> GetByStatusAsync(BatchStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingBatches
            .AsNoTracking()
            .Include(b => b.Program)
            .Include(b => b.Coach)
            .Where(b => b.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsBatchCodeUniqueAsync(string batchCode, CancellationToken cancellationToken = default)
    {
        return !await Context.TrainingBatches
            .AsNoTracking()
            .AnyAsync(b => b.BatchCode == batchCode, cancellationToken);
    }
}
