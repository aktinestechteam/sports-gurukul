using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAcademyFacilityRepository : IRepository<AcademyFacility>
{
    Task<IReadOnlyList<AcademyFacility>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyFacility>> GetByAcademyIdAndTypeAsync(Guid academyId, Domain.Enums.AcademyFacilityType facilityType, CancellationToken cancellationToken = default);
}
