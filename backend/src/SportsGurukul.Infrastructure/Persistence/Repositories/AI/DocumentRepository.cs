using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class DocumentRepository : Repository<KnowledgeDocument>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<KnowledgeDocument?> GetByIdWithChunksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeDocument>()
            .AsNoTracking()
            .Include(d => d.Chunks.OrderBy(c => c.ChunkIndex))
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> GetByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.KnowledgeBaseId == knowledgeBaseId)
            .OrderBy(d => d.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> GetBySourceAsync(Guid knowledgeSourceId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.KnowledgeSourceId == knowledgeSourceId)
            .OrderBy(d => d.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> GetByStatusAsync(AIDocumentStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.Status == status)
            .OrderBy(d => d.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<KnowledgeDocument?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeDocument>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ContentHash == contentHash, cancellationToken);
    }
}
