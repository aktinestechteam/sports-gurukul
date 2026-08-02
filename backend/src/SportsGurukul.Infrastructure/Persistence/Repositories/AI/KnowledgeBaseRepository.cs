using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class KnowledgeBaseRepository : Repository<Domain.Entities.AI.KnowledgeBase>, IKnowledgeBaseRepository
{
    public KnowledgeBaseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.KnowledgeBase?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeBase>()
            .AsNoTracking()
            .Include(k => k.Sources)
            .AsSplitQuery()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeBase>> GetByVisibilityAsync(string visibility, CancellationToken cancellationToken = default)
    {
        var vis = Enum.Parse<KnowledgeBaseVisibility>(visibility);
        return await Context.Set<Domain.Entities.AI.KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.Visibility == vis)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeBase>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var knowledgeBaseStatus = Enum.Parse<KnowledgeBaseStatus>(status);
        return await Context.Set<Domain.Entities.AI.KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.Status == knowledgeBaseStatus)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeBase>> GetPublicAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeBase>()
            .AsNoTracking()
            .Where(k => k.Visibility == KnowledgeBaseVisibility.Public)
            .ToListAsync(cancellationToken);
    }
}
