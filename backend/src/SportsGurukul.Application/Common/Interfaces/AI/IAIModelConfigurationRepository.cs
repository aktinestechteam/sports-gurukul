using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIModelConfigurationRepository : IRepository<AIModelConfiguration>
{
    Task<AIModelConfiguration?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIModelConfiguration>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default);
    Task<AIModelConfiguration?> GetDefaultForModelAsync(Guid modelId, CancellationToken cancellationToken = default);
}
