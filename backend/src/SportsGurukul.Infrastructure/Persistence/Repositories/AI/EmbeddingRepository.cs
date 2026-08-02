using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class EmbeddingRepository : Repository<Domain.Entities.AI.Embedding>, IEmbeddingRepository
{
    public EmbeddingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.Embedding?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Embedding>()
            .AsNoTracking()
            .Include(e => e.Document)
            .Include(e => e.Chunk)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Embedding>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Embedding>()
            .AsNoTracking()
            .Where(e => e.DocumentId == documentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Embedding>> GetByModelNameAsync(string modelName, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Embedding>()
            .AsNoTracking()
            .Where(e => e.ModelName == modelName)
            .ToListAsync(cancellationToken);
    }
}
