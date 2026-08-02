using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class PromptVersionRepository : Repository<Domain.Entities.AI.PromptVersion>, IPromptVersionRepository
{
    public PromptVersionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.PromptVersion?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptVersion>()
            .AsNoTracking()
            .Include(v => v.PromptTemplate)
            .AsSplitQuery()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.PromptVersion>> GetByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptVersion>()
            .AsNoTracking()
            .Where(v => v.PromptTemplateId == templateId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.AI.PromptVersion?> GetLatestVersionAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptVersion>()
            .AsNoTracking()
            .Where(v => v.PromptTemplateId == templateId)
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
