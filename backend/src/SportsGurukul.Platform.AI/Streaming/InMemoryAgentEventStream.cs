using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Streaming;

public class InMemoryAgentEventStream : IAgentEventStream
{
    private const int MaxHistoryPerRun = 500;

    private readonly ConcurrentDictionary<Guid, Channel<AgentStreamEvent>> _channels = new();
    private readonly ConcurrentDictionary<Guid, List<AgentStreamEvent>> _history = new();
    private readonly ConcurrentDictionary<Guid, long> _sequences = new();
    private readonly ILogger<InMemoryAgentEventStream> _logger;

    public InMemoryAgentEventStream(ILogger<InMemoryAgentEventStream>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryAgentEventStream>.Instance;
    }

    public async IAsyncEnumerable<AgentStreamEvent> WatchAsync(Guid runId, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = _channels.GetOrAdd(runId, _ => CreateChannel(runId));

        if (_history.TryGetValue(runId, out var history))
        {
            List<AgentStreamEvent> snapshot;
            lock (history)
            {
                snapshot = new List<AgentStreamEvent>(history);
            }

            foreach (var entry in snapshot.OrderBy(e => e.Sequence))
            {
                yield return entry;
                if (entry.Type == StreamEventType.Done)
                {
                    yield break;
                }
            }
        }

        await foreach (var entry in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            yield return entry;
            if (entry.Type == StreamEventType.Done)
            {
                yield break;
            }
        }
    }

    public Task PublishAsync(AgentStreamEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        @event.Sequence = _sequences.AddOrUpdate(@event.RunId, 1, (_, current) => current + 1);
        @event.Timestamp = DateTime.UtcNow;

        var history = _history.GetOrAdd(@event.RunId, _ => new List<AgentStreamEvent>());
        lock (history)
        {
            history.Add(@event);
            if (history.Count > MaxHistoryPerRun)
            {
                history.RemoveRange(0, history.Count - MaxHistoryPerRun);
            }
        }

        if (_channels.TryGetValue(@event.RunId, out var channel) && !channel.Writer.TryWrite(@event))
        {
            _logger.LogDebug("Event for run '{RunId}' could not be written; channel is closed", @event.RunId);
        }

        return Task.CompletedTask;
    }

    public async Task CompleteAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        await PublishAsync(AgentStreamEvent.Done(runId), cancellationToken);

        if (_channels.TryGetValue(runId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }

    private static Channel<AgentStreamEvent> CreateChannel(Guid runId)
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        return Channel.CreateUnbounded<AgentStreamEvent>(options);
    }
}
