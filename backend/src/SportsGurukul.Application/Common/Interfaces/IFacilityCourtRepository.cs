using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFacilityCourtRepository : IRepository<FacilityCourt>
{
    Task<IReadOnlyList<FacilityCourt>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<bool> IsCourtNumberUniqueInFacilityAsync(
        Guid facilityId, string courtNumber, CancellationToken cancellationToken = default);
}
