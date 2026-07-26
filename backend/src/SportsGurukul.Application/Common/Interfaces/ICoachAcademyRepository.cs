using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachAcademyRepository : IRepository<CoachAcademy>
{
    Task<IReadOnlyList<CoachAcademy>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachAcademy>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<CoachAcademy?> GetByAcademyAndCoachAsync(Guid academyId, Guid coachId, CancellationToken cancellationToken = default);
}
