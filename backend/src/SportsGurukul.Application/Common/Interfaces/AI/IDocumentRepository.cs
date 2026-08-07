using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IDocumentRepository : IRepository<KnowledgeDocument>
{
    Task<KnowledgeDocument?> GetByIdWithChunksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetBySourceAsync(Guid knowledgeSourceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetByStatusAsync(AIDocumentStatus status, CancellationToken cancellationToken = default);
    Task<KnowledgeDocument?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default);
}
