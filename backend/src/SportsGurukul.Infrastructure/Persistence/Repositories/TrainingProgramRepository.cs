using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class TrainingProgramRepository : Repository<TrainingProgram>, ITrainingProgramRepository
{
    public TrainingProgramRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TrainingProgram?> GetByProgramCodeAsync(string programCode, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProgramCode == programCode, cancellationToken);
    }

    public async Task<TrainingProgram?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingPrograms
            .AsNoTracking()
            .Include(p => p.Sport)
            .Include(p => p.Academy)
            .Include(p => p.ProgramSports).ThenInclude(ps => ps.Sport)
            .Include(p => p.Batches).ThenInclude(b => b.Coach)
            .Include(p => p.Goals)
            .Include(p => p.Milestones)
            .Include(p => p.Materials)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingProgram>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingPrograms
            .AsNoTracking()
            .Include(p => p.Sport)
            .Where(p => p.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingProgram>> GetBySportIdAsync(Guid sportId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingPrograms
            .AsNoTracking()
            .Include(p => p.Academy)
            .Where(p => p.SportId == sportId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingProgram>> GetByStatusAsync(TrainingProgramStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingPrograms
            .AsNoTracking()
            .Include(p => p.Sport)
            .Include(p => p.Academy)
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsProgramCodeUniqueAsync(string programCode, CancellationToken cancellationToken = default)
    {
        return !await Context.TrainingPrograms
            .AsNoTracking()
            .AnyAsync(p => p.ProgramCode == programCode, cancellationToken);
    }
}
