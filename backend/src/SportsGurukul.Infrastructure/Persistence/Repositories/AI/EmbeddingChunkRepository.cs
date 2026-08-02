using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class EmbeddingChunkRepository : Repository<Domain.Entities.AI.EmbeddingChunk>, IEmbeddingChunkRepository
{
    public EmbeddingChunkRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.EmbeddingChunk?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.EmbeddingChunk>()
            .AsNoTracking()
            .Include(c => c.Document)
            .Include(c => c.Embedding)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.EmbeddingChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.EmbeddingChunk>()
            .AsNoTracking()
            .Where(c => c.DocumentId == documentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.EmbeddingChunk>> GetByChunkIndexAsync(Guid documentId, int startIndex, int endIndex, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.EmbeddingChunk>()
            .AsNoTracking()
            .Where(c => c.DocumentId == documentId && c.ChunkIndex >= startIndex && c.ChunkIndex <= endIndex)
            .OrderBy(c => c.ChunkIndex)
            .ToListAsync(cancellationToken);
    }
}
