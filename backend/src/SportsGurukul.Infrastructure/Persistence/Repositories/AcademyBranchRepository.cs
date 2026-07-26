using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AcademyBranchRepository : Repository<AcademyBranch>, IAcademyBranchRepository
{
    public AcademyBranchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AcademyBranch>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyBranches
            .AsNoTracking()
            .Where(b => b.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<AcademyBranch?> GetByAcademyIdAndNameAsync(
        Guid academyId, string branchName, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyBranches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.AcademyId == academyId && b.BranchName == branchName, cancellationToken);
    }
}
