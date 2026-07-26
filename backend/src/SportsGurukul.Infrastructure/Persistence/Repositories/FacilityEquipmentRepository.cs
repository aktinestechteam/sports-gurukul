using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FacilityEquipmentRepository : Repository<FacilityEquipment>, IFacilityEquipmentRepository
{
    public FacilityEquipmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<FacilityEquipment>> GetByFacilityIdAsync(
        Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.FacilityEquipment
            .AsNoTracking()
            .Where(e => e.FacilityId == facilityId && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<FacilityEquipment?> GetWithMaintenanceAsync(
        Guid equipmentId, CancellationToken cancellationToken = default)
    {
        return await Context.FacilityEquipment
            .AsNoTracking()
            .Include(e => e.MaintenanceRecords.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == equipmentId && !e.IsDeleted, cancellationToken);
    }
}
