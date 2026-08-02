using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class KnowledgeSourceRepository : Repository<Domain.Entities.AI.KnowledgeSource>, IKnowledgeSourceRepository
{
    public KnowledgeSourceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.KnowledgeSource?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeSource>()
            .AsNoTracking()
            .Include(s => s.KnowledgeBase)
            .Include(s => s.Documents)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeSource>> GetByKnowledgeBaseIdAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeSource>()
            .AsNoTracking()
            .Where(s => s.KnowledgeBaseId == knowledgeBaseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeSource>> GetBySourceTypeAsync(string sourceType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<KnowledgeSourceType>(sourceType);
        return await Context.Set<Domain.Entities.AI.KnowledgeSource>()
            .AsNoTracking()
            .Where(s => s.SourceType == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeSource>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var sourceStatus = Enum.Parse<SourceStatus>(status);
        return await Context.Set<Domain.Entities.AI.KnowledgeSource>()
            .AsNoTracking()
            .Where(s => s.Status == sourceStatus)
            .ToListAsync(cancellationToken);
    }
}
