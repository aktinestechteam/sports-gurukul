using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachAvailabilityRepository : IRepository<CoachAvailability>
{
    Task<CoachAvailability?> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
}
