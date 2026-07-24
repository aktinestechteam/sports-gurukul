using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachRepository : IRepository<Coach>
{
    Task<Coach?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Coach?> GetByCoachCodeAsync(string coachCode, CancellationToken cancellationToken = default);
    Task<Coach?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Coach?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachSport>> GetCoachSportsAsync(Guid coachId, CancellationToken cancellationToken = default);
}
