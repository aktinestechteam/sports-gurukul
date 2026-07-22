using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default);
}
