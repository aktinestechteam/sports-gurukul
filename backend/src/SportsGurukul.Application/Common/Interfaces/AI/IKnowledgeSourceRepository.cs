using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IKnowledgeSourceRepository : IRepository<KnowledgeSource>
{
    Task<KnowledgeSource?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeSource>> GetByKnowledgeBaseIdAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeSource>> GetBySourceTypeAsync(string sourceType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeSource>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
