using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IPromptVersionRepository : IRepository<PromptVersion>
{
    Task<PromptVersion?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptVersion>> GetByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<PromptVersion?> GetLatestVersionAsync(Guid templateId, CancellationToken cancellationToken = default);
}
