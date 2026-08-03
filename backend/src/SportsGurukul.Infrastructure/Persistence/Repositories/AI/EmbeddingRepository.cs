using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class EmbeddingRepository : Repository<Embedding>, IEmbeddingRepository
{
    public EmbeddingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Embedding?> GetByChunkIdAsync(Guid chunkId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Embedding>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ChunkId == chunkId, cancellationToken);
    }

    public async Task<IReadOnlyList<Embedding>> GetByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Embedding>()
            .AsNoTracking()
            .Where(e => e.KnowledgeBaseId == knowledgeBaseId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Embedding>> GetByModelAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Embedding>()
            .AsNoTracking()
            .Where(e => e.ModelId == modelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Embedding>> GetByStatusAsync(AIEmbeddingStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Embedding>()
            .AsNoTracking()
            .Where(e => e.Status == status)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Embedding>()
            .AsNoTracking()
            .CountAsync(e => e.KnowledgeBaseId == knowledgeBaseId, cancellationToken);
    }
}
