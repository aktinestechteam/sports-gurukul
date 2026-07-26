using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AcademyFacilityRepository : Repository<AcademyFacility>, IAcademyFacilityRepository
{
    public AcademyFacilityRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AcademyFacility>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyFacilities
            .AsNoTracking()
            .Where(f => f.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyFacility>> GetByAcademyIdAndTypeAsync(
        Guid academyId, AcademyFacilityType facilityType, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyFacilities
            .AsNoTracking()
            .Where(f => f.AcademyId == academyId && f.FacilityType == facilityType)
            .ToListAsync(cancellationToken);
    }
}
