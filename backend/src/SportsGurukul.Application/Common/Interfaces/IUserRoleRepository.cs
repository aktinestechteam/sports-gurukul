using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IUserRoleRepository
{
    Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RemoveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
