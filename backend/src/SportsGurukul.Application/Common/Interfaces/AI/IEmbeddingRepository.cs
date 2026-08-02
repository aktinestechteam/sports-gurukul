using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IEmbeddingRepository : IRepository<Embedding>
{
    Task<Embedding?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Embedding>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Embedding>> GetByModelNameAsync(string modelName, CancellationToken cancellationToken = default);
}
