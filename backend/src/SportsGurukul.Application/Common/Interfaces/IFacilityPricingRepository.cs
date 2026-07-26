using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFacilityPricingRepository : IRepository<FacilityPricing>
{
    Task<IReadOnlyList<FacilityPricing>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
