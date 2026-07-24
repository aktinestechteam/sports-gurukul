using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachCertificationRepository : Repository<CoachCertification>, ICoachCertificationRepository
{
    public CoachCertificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CoachCertification>> GetByCoachIdAsync(
        Guid coachId, CancellationToken cancellationToken = default)
    {
        return await Context.CoachCertifications
            .AsNoTracking()
            .Where(c => c.CoachId == coachId)
            .OrderByDescending(c => c.IssueDate)
            .ToListAsync(cancellationToken);
    }
}
