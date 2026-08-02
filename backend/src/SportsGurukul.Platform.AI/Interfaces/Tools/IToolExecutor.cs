using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface IToolExecutor
{
    Task<ToolResult> ExecuteAsync(string toolName, IDictionary<string, object?> arguments, ToolExecutionContext context, CancellationToken cancellationToken = default);

    Task<ToolResult> ExecuteAsync(ToolCall call, ToolExecutionContext context, CancellationToken cancellationToken = default);
}
