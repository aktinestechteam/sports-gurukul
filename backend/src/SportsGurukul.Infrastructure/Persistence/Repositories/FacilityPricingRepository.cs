using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FacilityPricingRepository : Repository<FacilityPricing>, IFacilityPricingRepository
{
    public FacilityPricingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<FacilityPricing>> GetByFacilityIdAsync(
        Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.FacilityPricing
            .AsNoTracking()
            .Where(p => p.FacilityId == facilityId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
