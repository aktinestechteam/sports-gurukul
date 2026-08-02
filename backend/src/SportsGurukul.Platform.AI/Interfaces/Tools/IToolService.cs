using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface IToolService
{
    Task<ITool> RegisterAsync(ITool tool, CancellationToken cancellationToken = default);

    Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ToolDescriptor>> ListAsync(CancellationToken cancellationToken = default);

    Task<ToolDescriptor?> DescribeAsync(string name, CancellationToken cancellationToken = default);

    Task<ToolResult> ExecuteAsync(string toolName, IDictionary<string, object?> arguments, ToolExecutionContext context, CancellationToken cancellationToken = default);
}
