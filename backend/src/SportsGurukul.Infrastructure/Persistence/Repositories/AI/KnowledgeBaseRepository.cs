using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class KnowledgeBaseRepository : Repository<KnowledgeBase>, IKnowledgeBaseRepository
{
    public KnowledgeBaseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<KnowledgeBase?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeBase>()
            .AsNoTracking()
            .Include(k => k.VectorIndex)
            .Include(k => k.EmbeddingModel)
            .Include(k => k.Sources)
            .Include(k => k.Documents)
            .AsSplitQuery()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<KnowledgeBase?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeBase>()
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeBase>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.OwnerUserId == ownerUserId)
            .OrderBy(k => k.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeBase>> GetByTypeAsync(AIKnowledgeBaseType knowledgeBaseType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.KnowledgeBaseType == knowledgeBaseType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeBase>> GetByVectorIndexAsync(Guid vectorIndexId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.VectorIndexId == vectorIndexId)
            .ToListAsync(cancellationToken);
    }
}
