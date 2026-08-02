using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class KnowledgeDocumentRepository : Repository<Domain.Entities.AI.KnowledgeDocument>, IKnowledgeDocumentRepository
{
    public KnowledgeDocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.KnowledgeDocument?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeDocument>()
            .AsNoTracking()
            .Include(d => d.KnowledgeSource)
            .Include(d => d.Embeddings)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeDocument>> GetByKnowledgeSourceIdAsync(Guid knowledgeSourceId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.KnowledgeSourceId == knowledgeSourceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeDocument>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var embeddingStatus = Enum.Parse<EmbeddingStatus>(status);
        return await Context.Set<Domain.Entities.AI.KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.EmbeddingStatus == embeddingStatus)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.KnowledgeDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<DocumentType>(documentType);
        return await Context.Set<Domain.Entities.AI.KnowledgeDocument>()
            .AsNoTracking()
            .Where(d => d.Type == type)
            .ToListAsync(cancellationToken);
    }
}
