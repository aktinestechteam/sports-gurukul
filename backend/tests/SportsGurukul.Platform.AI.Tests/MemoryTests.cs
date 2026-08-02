using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.Interfaces.Memory;
using SportsGurukul.Platform.AI.Memory;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tests;

public class MemoryTests
{
    private readonly InMemoryMemoryStore _store = new(NullLogger<InMemoryMemoryStore>.Instance);
    private readonly HashedEmbeddingProvider _embeddings = new();
    private readonly MemoryService _service;

    public MemoryTests()
    {
        _service = new MemoryService(
            _store,
            _store,
            _store,
            _store,
            _store,
            _embeddings,
            NullLogger<MemoryService>.Instance);
    }

    [Fact]
    public async Task WriteAndRecallWorking()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Working,
            Subject = "task",
            Content = "Step one complete",
            SessionId = "s1"
        });

        var entries = await _service.RecallWorkingAsync("s1");

        Assert.Single(entries);
        Assert.Equal("Step one complete", entries[0].Content);
    }

    [Fact]
    public async Task WriteSessionAndClear()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Session,
            Subject = "chat",
            Content = "User asked about cricket",
            SessionId = "s1"
        });

        await _service.ClearSessionAsync("s1");

        Assert.Empty(await _service.Session.GetAsync("s1"));
    }

    [Fact]
    public async Task WriteLongTermAndRecall()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.LongTerm,
            Subject = "player",
            Content = "Player prefers left-handed bowling",
            TenantId = "t1"
        });

        var results = await _service.RecallAsync(new MemoryQuery { Subject = "player", TenantId = "t1" });

        Assert.Single(results);
        Assert.Contains("left-handed", results[0].Content);
    }

    [Fact]
    public async Task SemanticSearch_UsesEmbeddingSimilarity()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Semantic,
            Subject = "training",
            Content = "Focus on sprint drills for fielders"
        });
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Semantic,
            Subject = "finance",
            Content = "Budget review for academy"
        });

        var embedding = await _embeddings.EmbedAsync("sprint training drills");
        var results = await _service.Semantic.SearchAsync("sprint training drills", embedding, 5);

        Assert.Single(results);
        Assert.Contains("sprint", results[0].Content);
    }

    [Fact]
    public async Task Snapshot_ContainsWorkingAndSession()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Working,
            Subject = "w",
            Content = "working note",
            SessionId = "s9"
        });
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.Session,
            Subject = "s",
            Content = "session note",
            SessionId = "s9"
        });

        var snapshot = await _service.SnapshotAsync("s9");

        Assert.Single(snapshot.Working);
        Assert.Single(snapshot.Session);
    }

    [Fact]
    public async Task EmbeddingProvider_ProducesDeterministicVectors()
    {
        var a = await _embeddings.EmbedAsync("sprint training");
        var b = await _embeddings.EmbedAsync("sprint training");

        Assert.Equal(a.Count, b.Count);
        for (var i = 0; i < a.Count; i++)
        {
            Assert.Equal(a[i], b[i], 5);
        }
    }

    [Fact]
    public async Task MemoryStats_ReflectsStoreCounts()
    {
        await _service.WriteAsync(new MemoryEntry
        {
            Category = MemoryCategory.LongTerm,
            Subject = "note",
            Content = "persistent note"
        });

        var stats = await _service.GetStatsAsync();

        Assert.Equal(1, stats.LongTerm);
    }
}
