using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class PromptTemplateRepository : Repository<Domain.Entities.AI.PromptTemplate>, IPromptTemplateRepository
{
    public PromptTemplateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.PromptTemplate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptTemplate>()
            .AsNoTracking()
            .Include(t => t.Versions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.PromptTemplate>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.Status == PromptStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.PromptTemplate>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.Category == category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.PromptTemplate>> GetByTypeAsync(string templateType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<PromptType>(templateType);
        return await Context.Set<Domain.Entities.AI.PromptTemplate>()
            .AsNoTracking()
            .Where(t => t.Type == type)
            .ToListAsync(cancellationToken);
    }
}
