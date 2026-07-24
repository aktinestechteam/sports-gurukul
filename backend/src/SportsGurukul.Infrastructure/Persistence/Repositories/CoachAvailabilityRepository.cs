using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachAvailabilityRepository : Repository<CoachAvailability>, ICoachAvailabilityRepository
{
    public CoachAvailabilityRepository(ApplicationDbContext context) : base(context) { }

    public async Task<CoachAvailability?> GetByCoachIdAsync(
        Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachAvailabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CoachId == coachId, cancellationToken);
    }
}
