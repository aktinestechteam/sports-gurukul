using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAchievementRepository : IRepository<Achievement>
{
    Task<IReadOnlyList<Achievement>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<Achievement?> GetWithAthletesAsync(Guid achievementId, CancellationToken cancellationToken = default);
}
