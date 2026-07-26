using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AcademyRepository : Repository<Academy>, IAcademyRepository
{
    public AcademyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Academy?> GetByAcademyCodeAsync(string academyCode, CancellationToken cancellationToken = default)
    {
        return await Context.Academies
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AcademyCode == academyCode, cancellationToken);
    }

    public async Task<Academy?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Academies
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<Academy?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.OperatingHours)
            .Include(a => a.Verification)
            .Include(a => a.Branches)
            .Include(a => a.AcademySports).ThenInclude(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.Facilities)
            .Include(a => a.Memberships)
            .Include(a => a.Documents)
            .Include(a => a.GalleryImages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Academy?> GetByAcademyCodeWithDetailsAsync(string academyCode, CancellationToken cancellationToken = default)
    {
        return await Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.OperatingHours)
            .Include(a => a.Verification)
            .Include(a => a.Branches)
            .Include(a => a.AcademySports).ThenInclude(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.Facilities)
            .Include(a => a.Memberships)
            .Include(a => a.Documents)
            .Include(a => a.GalleryImages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.AcademyCode == academyCode, cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyBranch>> GetBranchesAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyBranches
            .AsNoTracking()
            .Where(b => b.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademySport>> GetAcademySportsAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademySports
            .AsNoTracking()
            .Include(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Where(s => s.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyFacility>> GetFacilitiesAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyFacilities
            .AsNoTracking()
            .Where(f => f.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyMembership>> GetMembershipsAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyMemberships
            .AsNoTracking()
            .Where(m => m.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyDocument>> GetDocumentsAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyDocuments
            .AsNoTracking()
            .Where(d => d.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademyGallery>> GetGalleryImagesAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademyGalleries
            .AsNoTracking()
            .Where(g => g.AcademyId == academyId)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
