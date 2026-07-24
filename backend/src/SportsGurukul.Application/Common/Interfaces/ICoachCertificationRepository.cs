using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachCertificationRepository : IRepository<CoachCertification>
{
    Task<IReadOnlyList<CoachCertification>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
}
