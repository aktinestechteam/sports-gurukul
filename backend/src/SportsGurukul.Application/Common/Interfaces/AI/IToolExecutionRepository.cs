using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IToolExecutionRepository : IRepository<ToolExecution>
{
    Task<ToolExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToolExecution>> GetByToolDefinitionIdAsync(Guid toolDefinitionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToolExecution>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToolExecution>> GetRecentAsync(Guid conversationId, int count, CancellationToken cancellationToken = default);
}
