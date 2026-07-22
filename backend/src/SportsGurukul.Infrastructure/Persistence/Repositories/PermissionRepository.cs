using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name && !p.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default)
    {
        return await Context.Permissions
            .AsNoTracking()
            .Where(p => p.Module == module && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
