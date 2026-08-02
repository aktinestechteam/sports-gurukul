using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IEmbeddingChunkRepository : IRepository<EmbeddingChunk>
{
    Task<EmbeddingChunk?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmbeddingChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmbeddingChunk>> GetByChunkIndexAsync(Guid documentId, int startIndex, int endIndex, CancellationToken cancellationToken = default);
}
