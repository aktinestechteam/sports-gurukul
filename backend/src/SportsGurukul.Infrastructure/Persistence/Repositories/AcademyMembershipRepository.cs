using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AcademyMembershipRepository : Repository<AcademyMembership>, IAcademyMembershipRepository
{
    public AcademyMembershipRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AcademyMembership>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyMemberships
            .AsNoTracking()
            .Where(m => m.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyMembership>> GetActiveByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyMemberships
            .AsNoTracking()
            .Where(m => m.AcademyId == academyId && m.Status == AcademyMembershipStatus.Active)
            .ToListAsync(cancellationToken);
    }
}
