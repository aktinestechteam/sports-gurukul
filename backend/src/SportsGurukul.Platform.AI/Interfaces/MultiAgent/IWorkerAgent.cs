using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.MultiAgent;

public interface IWorkerAgent
{
    string Name { get; }

    IReadOnlyList<string> Capabilities { get; }

    Task<DelegatedTaskResult> ExecuteAsync(DelegatedTask task, CancellationToken cancellationToken = default);
}
