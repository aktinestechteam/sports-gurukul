using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IKnowledgeBaseRepository : IRepository<KnowledgeBase>
{
    Task<KnowledgeBase?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<KnowledgeBase?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetByTypeAsync(AIKnowledgeBaseType knowledgeBaseType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetByVectorIndexAsync(Guid vectorIndexId, CancellationToken cancellationToken = default);
}
