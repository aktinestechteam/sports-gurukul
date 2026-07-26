using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachAcademyRepository : Repository<CoachAcademy>, ICoachAcademyRepository
{
    public CoachAcademyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CoachAcademy>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachAcademies
            .AsNoTracking()
            .Where(ca => ca.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CoachAcademy>> GetByCoachIdAsync(
        Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachAcademies
            .AsNoTracking()
            .Where(ca => ca.CoachId == coachId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CoachAcademy?> GetByAcademyAndCoachAsync(
        Guid academyId, Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachAcademies
            .AsNoTracking()
            .FirstOrDefaultAsync(ca => ca.AcademyId == academyId && ca.CoachId == coachId, cancellationToken);
    }
}
