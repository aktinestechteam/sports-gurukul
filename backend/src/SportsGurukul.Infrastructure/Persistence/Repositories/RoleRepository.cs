using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted, cancellationToken);
    }

    public async Task<Role?> GetByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken = default)
    {
        return await Context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleType == roleType && !r.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
