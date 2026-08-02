using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IKnowledgeBaseRepository : IRepository<KnowledgeBase>
{
    Task<KnowledgeBase?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetByVisibilityAsync(string visibility, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeBase>> GetPublicAsync(CancellationToken cancellationToken = default);
}
