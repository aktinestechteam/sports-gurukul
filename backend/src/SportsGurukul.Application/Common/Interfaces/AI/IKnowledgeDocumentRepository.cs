using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IKnowledgeDocumentRepository : IRepository<KnowledgeDocument>
{
    Task<KnowledgeDocument?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetByKnowledgeSourceIdAsync(Guid knowledgeSourceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
}
