using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IEmbeddingRepository : IRepository<Embedding>
{
    Task<Embedding?> GetByChunkIdAsync(Guid chunkId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Embedding>> GetByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Embedding>> GetByModelAsync(Guid modelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Embedding>> GetByStatusAsync(AIEmbeddingStatus status, CancellationToken cancellationToken = default);
    Task<long> CountByKnowledgeBaseAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);
}
