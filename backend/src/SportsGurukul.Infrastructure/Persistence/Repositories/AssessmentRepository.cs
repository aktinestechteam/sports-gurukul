using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AssessmentRepository : Repository<TrainingAssessment>, IAssessmentRepository
{
    public AssessmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TrainingAssessment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingAssessments
            .AsNoTracking()
            .Include(a => a.Session)
            .Include(a => a.Results).ThenInclude(r => r.Athlete)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingAssessment>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingAssessments
            .AsNoTracking()
            .Include(a => a.Results)
            .Where(a => a.SessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrainingAssessment>> GetByTypeAsync(AssessmentType type, CancellationToken cancellationToken = default)
    {
        return await Context.TrainingAssessments
            .AsNoTracking()
            .Include(a => a.Session)
            .Where(a => a.AssessmentType == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssessmentResult>> GetResultsByAssessmentIdAsync(Guid assessmentId, CancellationToken cancellationToken = default)
    {
        return await Context.AssessmentResults
            .AsNoTracking()
            .Include(r => r.Athlete)
            .Where(r => r.AssessmentId == assessmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssessmentResult>> GetResultsByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AssessmentResults
            .AsNoTracking()
            .Include(r => r.Assessment).ThenInclude(a => a!.Session)
            .Where(r => r.AthleteId == athleteId)
            .OrderByDescending(r => r.AssessedAt)
            .ToListAsync(cancellationToken);
    }
}
