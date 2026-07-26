using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FacilityCourtRepository : Repository<FacilityCourt>, IFacilityCourtRepository
{
    public FacilityCourtRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<FacilityCourt>> GetByFacilityIdAsync(
        Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.FacilityCourts
            .AsNoTracking()
            .Where(c => c.FacilityId == facilityId && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCourtNumberUniqueInFacilityAsync(
        Guid facilityId, string courtNumber, CancellationToken cancellationToken = default)
    {
        return await Context.FacilityCourts
            .AsNoTracking()
            .AnyAsync(c =>
                c.FacilityId == facilityId &&
                c.CourtNumber == courtNumber &&
                !c.IsDeleted, cancellationToken);
    }
}
