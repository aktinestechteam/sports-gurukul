using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Streaming;

public interface IAgentEventStream
{
    IAsyncEnumerable<AgentStreamEvent> WatchAsync(Guid runId, CancellationToken cancellationToken = default);

    Task PublishAsync(AgentStreamEvent @event, CancellationToken cancellationToken = default);

    Task CompleteAsync(Guid runId, CancellationToken cancellationToken = default);
}
