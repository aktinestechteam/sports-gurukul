using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class PromptRepository : Repository<PromptTemplate>, IPromptRepository
{
    public PromptRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PromptTemplate?> GetByIdWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .Include(t => t.Versions.OrderByDescending(v => v.VersionNumber))
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<PromptTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<PromptTemplate>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.AssistantId == assistantId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromptTemplate>> GetActiveByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.AssistantId == assistantId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PromptTemplate?> GetDefaultByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.AssistantId == assistantId && t.IsActive && t.IsDefault)
            .OrderBy(t => t.CurrentVersion)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromptTemplate>> GetByTypeAsync(AIPromptType promptType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.PromptType == promptType)
            .ToListAsync(cancellationToken);
    }
}
