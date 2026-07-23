using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ISavedSearchRepository : IRepository<SavedSearch>
{
    Task<IReadOnlyList<SavedSearch>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SavedSearch?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
