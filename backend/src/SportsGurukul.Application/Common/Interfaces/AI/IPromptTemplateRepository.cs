using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IPromptTemplateRepository : IRepository<PromptTemplate>
{
    Task<PromptTemplate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetByTypeAsync(string templateType, CancellationToken cancellationToken = default);
}
