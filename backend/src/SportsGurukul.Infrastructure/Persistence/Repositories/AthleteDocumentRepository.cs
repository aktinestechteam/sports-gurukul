using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AthleteDocumentRepository : Repository<AthleteDocument>, IAthleteDocumentRepository
{
    public AthleteDocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AthleteDocument>> GetByAthleteIdAsync(
        Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteDocuments
            .AsNoTracking()
            .Where(d => d.AthleteId == athleteId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<AthleteDocument?> GetByIdWithDetailsAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteDocuments
            .IgnoreQueryFilters()
            .Include(d => d.Versions)
            .Include(d => d.AuditTrail.OrderByDescending(a => a.PerformedOn))
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentVersion>> GetVersionsAsync(
        Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.DocumentVersions
            .AsNoTracking()
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentAudit>> GetAuditTrailAsync(
        Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.DocumentAudits
            .AsNoTracking()
            .Where(a => a.DocumentId == documentId)
            .OrderByDescending(a => a.PerformedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxVersionNumberAsync(
        Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.DocumentVersions
            .AsNoTracking()
            .Where(v => v.DocumentId == documentId)
            .MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0;
    }

    public async Task AddVersionAsync(DocumentVersion version, CancellationToken cancellationToken = default)
    {
        await Context.DocumentVersions.AddAsync(version, cancellationToken);
    }

    public async Task AddAuditAsync(DocumentAudit audit, CancellationToken cancellationToken = default)
    {
        await Context.DocumentAudits.AddAsync(audit, cancellationToken);
    }
}
