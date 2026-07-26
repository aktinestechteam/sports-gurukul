using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FacilityRepository : Repository<Facility>, IFacilityRepository
{
    public FacilityRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Facility>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.Facilities
            .AsNoTracking()
            .Where(f => f.AcademyId == academyId && !f.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Facility?> GetWithDetailsAsync(
        Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.Facilities
            .AsNoTracking()
            .Include(f => f.Courts.Where(c => !c.IsDeleted))
            .Include(f => f.Equipment.Where(e => !e.IsDeleted))
            .Include(f => f.Schedules.Where(s => !s.IsDeleted))
            .Include(f => f.PricingTiers.Where(p => !p.IsDeleted))
            .Include(f => f.Images.Where(i => !i.IsDeleted))
            .Include(f => f.Amenities.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(f => f.Id == facilityId && !f.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Facility>> SearchAsync(
        Guid? academyId,
        FacilityType? facilityType,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Facilities
            .AsNoTracking()
            .Where(f => !f.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(f => f.AcademyId == academyId.Value);

        if (facilityType.HasValue)
            query = query.Where(f => f.FacilityType == facilityType.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(f =>
                f.FacilityName.Contains(searchTerm) ||
                f.FacilityCode.Contains(searchTerm) ||
                (f.Description != null && f.Description.Contains(searchTerm)));

        return await query
            .OrderBy(f => f.FacilityName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? academyId,
        FacilityType? facilityType,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Facilities
            .AsNoTracking()
            .Where(f => !f.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(f => f.AcademyId == academyId.Value);

        if (facilityType.HasValue)
            query = query.Where(f => f.FacilityType == facilityType.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(f =>
                f.FacilityName.Contains(searchTerm) ||
                f.FacilityCode.Contains(searchTerm) ||
                (f.Description != null && f.Description.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsFacilityNameUniqueInBranchAsync(
        Guid academyId, Guid? branchId, string facilityName, CancellationToken cancellationToken = default)
    {
        return await Context.Facilities
            .AsNoTracking()
            .AnyAsync(f =>
                f.AcademyId == academyId &&
                f.BranchId == branchId &&
                f.FacilityName == facilityName &&
                !f.IsDeleted, cancellationToken);
    }
}
