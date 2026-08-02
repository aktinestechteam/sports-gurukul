using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface ITool
{
    string Name { get; }

    string? Description { get; }

    ToolType Type { get; }

    bool RequiresApproval { get; }

    int? TimeoutSeconds { get; }

    string? Permission { get; }

    IReadOnlyList<string> Tags { get; }

    IReadOnlyDictionary<string, string> Parameters { get; }

    Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken = default);
}
