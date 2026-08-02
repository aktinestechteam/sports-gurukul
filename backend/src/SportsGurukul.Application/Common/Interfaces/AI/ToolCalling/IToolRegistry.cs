using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.ToolCalling;

public interface IToolRegistry
{
    Task<Result<ToolDefinition>> RegisterAsync(ToolDefinition tool, CancellationToken cancellationToken = default);
    Task<Result<bool>> UnregisterAsync(Guid toolId, CancellationToken cancellationToken = default);
    Task<Result<ToolDefinition>> GetByIdAsync(Guid toolId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ToolDefinition>>> GetActiveToolsAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ToolDefinition>>> GetToolsByTypeAsync(ToolType toolType, CancellationToken cancellationToken = default);
}
