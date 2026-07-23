using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IRecentSearchRepository : IRepository<RecentSearch>
{
    Task<IReadOnlyList<RecentSearch>> GetByUserIdAsync(Guid userId, int limit = 10, CancellationToken cancellationToken = default);
    Task DeleteOlderThanAsync(Guid userId, int keepCount, CancellationToken cancellationToken = default);
}
