using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.AI.ToolCalling;

public interface IToolExecutor
{
    Task<Result<string>> ExecuteAsync(Guid toolId, string input, CancellationToken cancellationToken = default);
    Task<Result<string>> ExecuteWithContextAsync(Guid toolId, string input, Guid? conversationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateAsync(Guid toolId, string input, CancellationToken cancellationToken = default);
}
