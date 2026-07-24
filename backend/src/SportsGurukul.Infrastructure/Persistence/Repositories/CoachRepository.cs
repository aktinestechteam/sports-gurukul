using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachRepository : Repository<Coach>, ICoachRepository
{
    public CoachRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Coach?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Coaches
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Coach?> GetByCoachCodeAsync(string coachCode, CancellationToken cancellationToken = default)
    {
        return await Context.Coaches
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CoachCode == coachCode, cancellationToken);
    }

    public async Task<Coach?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Coaches
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.User.UserRoles).ThenInclude(ur => ur.Role)
            .Include(c => c.CoachSports).ThenInclude(cs => cs.Sport).ThenInclude(s => s!.SportCategory)
            .Include(c => c.Certifications)
            .Include(c => c.Experiences)
            .Include(c => c.Education)
            .Include(c => c.Specializations)
            .Include(c => c.Documents)
            .Include(c => c.Availability)
            .Include(c => c.Location)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Coach?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Coaches
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.User.UserRoles).ThenInclude(ur => ur.Role)
            .Include(c => c.CoachSports).ThenInclude(cs => cs.Sport).ThenInclude(s => s!.SportCategory)
            .Include(c => c.Certifications)
            .Include(c => c.Experiences)
            .Include(c => c.Education)
            .Include(c => c.Specializations)
            .Include(c => c.Documents)
            .Include(c => c.Availability)
            .Include(c => c.Location)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<CoachSport>> GetCoachSportsAsync(Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachSports
            .AsNoTracking()
            .Include(cs => cs.Sport).ThenInclude(s => s!.SportCategory)
            .Where(cs => cs.CoachId == coachId)
            .ToListAsync(cancellationToken);
    }
}
