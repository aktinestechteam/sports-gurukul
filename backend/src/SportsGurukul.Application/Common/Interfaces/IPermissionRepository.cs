using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
}
