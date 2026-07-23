using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ISportRepository : IRepository<Sport>
{
    Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Sport?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sport>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
