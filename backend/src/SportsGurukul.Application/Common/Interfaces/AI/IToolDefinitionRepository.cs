using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IToolDefinitionRepository : IRepository<ToolDefinition>
{
    Task<ToolDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToolDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToolDefinition>> GetByTypeAsync(string toolType, CancellationToken cancellationToken = default);
}
