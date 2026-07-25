using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachDocumentRepository : Repository<CoachDocument>, ICoachDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public CoachDocumentRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CoachDocument>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default)
    {
        return await _context.CoachDocuments
            .AsNoTracking()
            .Where(d => d.CoachId == coachId && !d.IsDeleted)
            .Include(d => d.Versions)
            .Include(d => d.AuditTrail)
            .OrderByDescending(d => d.UploadedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<CoachDocument?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CoachDocuments
            .IgnoreQueryFilters()
            .Include(d => d.Versions)
            .Include(d => d.AuditTrail.OrderByDescending(a => a.PerformedOn))
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CoachDocumentVersion>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await _context.CoachDocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CoachDocumentAudit>> GetAuditTrailAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await _context.CoachDocumentAudits
            .Where(a => a.DocumentId == documentId)
            .OrderByDescending(a => a.PerformedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxVersionNumberAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await _context.CoachDocumentVersions
            .Where(v => v.DocumentId == documentId)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync(cancellationToken) ?? 0;
    }

    public async Task AddVersionAsync(CoachDocumentVersion version, CancellationToken cancellationToken = default)
    {
        await _context.CoachDocumentVersions.AddAsync(version, cancellationToken);
    }

    public async Task AddAuditAsync(CoachDocumentAudit audit, CancellationToken cancellationToken = default)
    {
        await _context.CoachDocumentAudits.AddAsync(audit, cancellationToken);
    }
}
