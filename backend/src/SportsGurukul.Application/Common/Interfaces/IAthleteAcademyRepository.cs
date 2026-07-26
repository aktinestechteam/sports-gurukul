using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAthleteAcademyRepository : IRepository<AthleteAcademy>
{
    Task<IReadOnlyList<AthleteAcademy>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AthleteAcademy>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<AthleteAcademy?> GetByAcademyAndAthleteAsync(Guid academyId, Guid athleteId, CancellationToken cancellationToken = default);
}
