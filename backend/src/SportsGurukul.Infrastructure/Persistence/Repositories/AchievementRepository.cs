using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AchievementRepository : Repository<Achievement>, IAchievementRepository
{
    public AchievementRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Achievement>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Achievements
            .AsNoTracking()
            .Where(a => a.AthleteAchievements.Any(aa => aa.AthleteId == athleteId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Achievement?> GetWithAthletesAsync(Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await Context.Achievements
            .AsNoTracking()
            .Include(a => a.AthleteAchievements).ThenInclude(aa => aa.Athlete)
            .FirstOrDefaultAsync(a => a.Id == achievementId, cancellationToken);
    }
}
