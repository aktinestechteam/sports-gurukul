using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Models;
using SportsGurukul.Platform.AI.Streaming;

namespace SportsGurukul.Platform.AI.Tests;

public class StreamingTests
{
    [Fact]
    public async Task Watch_ReplaysHistoryAndLiveEventsInOrder()
    {
        var stream = new InMemoryAgentEventStream();
        var runId = Guid.NewGuid();

        await stream.PublishAsync(AgentStreamEvent.Status(runId, "Running"));
        await stream.PublishAsync(AgentStreamEvent.Plan(runId, "plan step"));

        var received = new List<StreamEventType>();
        var watch = Task.Run(async () =>
        {
            await foreach (var e in stream.WatchAsync(runId))
            {
                received.Add(e.Type);
                if (received.Count >= 3)
                {
                    break;
                }
            }
        });

        await Task.Delay(200);
        await stream.PublishAsync(AgentStreamEvent.ToolCall(runId, "database"));

        await watch.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal([StreamEventType.Status, StreamEventType.Plan, StreamEventType.ToolCall], received);
    }

    [Fact]
    public async Task Watch_CompletesOnDone()
    {
        var stream = new InMemoryAgentEventStream();
        var runId = Guid.NewGuid();

        var watchTask = Task.Run(async () =>
        {
            var events = new List<StreamEventType>();
            await foreach (var e in stream.WatchAsync(runId))
            {
                events.Add(e.Type);
            }

            return events;
        });

        await Task.Delay(200);
        await stream.PublishAsync(AgentStreamEvent.Status(runId, "Running"));
        await stream.CompleteAsync(runId);

        var events = await watchTask.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal([StreamEventType.Status, StreamEventType.Done], events);
    }

    [Fact]
    public async Task Publish_AssignsSequentialNumbers()
    {
        var stream = new InMemoryAgentEventStream();
        var runId = Guid.NewGuid();

        await stream.PublishAsync(AgentStreamEvent.Status(runId, "a"));
        await stream.PublishAsync(AgentStreamEvent.Message(runId, "b"));
        await stream.PublishAsync(AgentStreamEvent.ToolCall(runId, "c"));

        var watchTask = Task.Run(async () =>
        {
            var events = new List<AgentStreamEvent>();
            await foreach (var e in stream.WatchAsync(runId))
            {
                events.Add(e);
                if (events.Count >= 3)
                {
                    break;
                }
            }

            return events;
        });

        var events = await watchTask.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal([1, 2, 3], events.Select(e => e.Sequence));
    }

    [Fact]
    public async Task Watch_RespectsCancellation()
    {
        var stream = new InMemoryAgentEventStream();
        var runId = Guid.NewGuid();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

        await stream.PublishAsync(AgentStreamEvent.Status(runId, "Running"));

        var completed = false;
        var watchTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var _ in stream.WatchAsync(runId, cts.Token))
                {
                }
            }
            catch (OperationCanceledException)
            {
            }

            completed = true;
        });

        await watchTask.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True(completed);
    }
}
