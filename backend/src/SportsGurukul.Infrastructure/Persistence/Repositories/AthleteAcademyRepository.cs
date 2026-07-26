using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AthleteAcademyRepository : Repository<AthleteAcademy>, IAthleteAcademyRepository
{
    public AthleteAcademyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AthleteAcademy>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteAcademies
            .AsNoTracking()
            .Where(aa => aa.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AthleteAcademy>> GetByAthleteIdAsync(
        Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteAcademies
            .AsNoTracking()
            .Where(aa => aa.AthleteId == athleteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<AthleteAcademy?> GetByAcademyAndAthleteAsync(
        Guid academyId, Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteAcademies
            .AsNoTracking()
            .FirstOrDefaultAsync(aa => aa.AcademyId == academyId && aa.AthleteId == athleteId, cancellationToken);
    }
}
