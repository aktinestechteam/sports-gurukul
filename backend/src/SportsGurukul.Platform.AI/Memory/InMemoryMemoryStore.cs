using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Memory;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Memory;

public class InMemoryMemoryStore :
    IWorkingMemoryStore,
    ISessionMemoryStore,
    ILongTermMemoryStore,
    ISemanticMemoryStore,
    IEpisodicMemoryStore
{
    private readonly ConcurrentDictionary<Guid, MemoryEntry> _working = new();
    private readonly ConcurrentDictionary<Guid, MemoryEntry> _session = new();
    private readonly ConcurrentDictionary<Guid, MemoryEntry> _longTerm = new();
    private readonly ConcurrentDictionary<Guid, MemoryEntry> _semantic = new();
    private readonly ConcurrentDictionary<Guid, MemoryEntry> _episodic = new();
    private readonly ILogger<InMemoryMemoryStore> _logger;

    internal const double MinimumSimilarity = 0.1;

    public InMemoryMemoryStore(ILogger<InMemoryMemoryStore>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryMemoryStore>.Instance;
    }

    public Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var target = entry.Category switch
        {
            MemoryCategory.Working => _working,
            MemoryCategory.Session => _session,
            MemoryCategory.LongTerm => _longTerm,
            MemoryCategory.Semantic => _semantic,
            MemoryCategory.Episodic => _episodic,
            _ => _longTerm
        };

        target[entry.Id] = entry;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<MemoryEntry>> GetAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entries = _session.Values.Where(e => e.SessionId == sessionId).OrderByDescending(e => e.CreatedAt).ToList();
        return Task.FromResult<IReadOnlyList<MemoryEntry>>(entries);
    }

    Task<IReadOnlyList<MemoryEntry>> IWorkingMemoryStore.GetAsync(string sessionId, string? tenantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entries = _working.Values
            .Where(e => e.SessionId == sessionId)
            .Where(e => tenantId is null || e.TenantId == tenantId)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
        return Task.FromResult<IReadOnlyList<MemoryEntry>>(entries);
    }

    public Task ClearAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var (id, entry) in _session)
        {
            if (entry.SessionId == sessionId)
            {
                _session.TryRemove(id, out _);
            }
        }

        return Task.CompletedTask;
    }

    Task IWorkingMemoryStore.ClearAsync(string sessionId, string? tenantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var (id, entry) in _working)
        {
            if (entry.SessionId == sessionId)
            {
                _working.TryRemove(id, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(Guid entryId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_working.TryRemove(entryId, out _));
    }

    public Task<IReadOnlyList<MemoryEntry>> FindAsync(string subject, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entries = _longTerm.Values
            .Where(e => e.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase))
            .Where(e => tenantId is null || e.TenantId == tenantId)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
        return Task.FromResult<IReadOnlyList<MemoryEntry>>(entries);
    }

    public Task<IReadOnlyList<MemoryEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<MemoryEntry>>(_longTerm.Values.OrderByDescending(e => e.CreatedAt).ToList());
    }

    public Task<IReadOnlyList<MemoryEntry>> SearchAsync(string subject, IReadOnlyList<float>? embedding, int limit, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<MemoryEntry> candidates = _semantic.Values;

        if (embedding is not null && embedding.Count > 0)
        {
            candidates = candidates
                .Where(e => e.Embedding is not null)
                .Select(e => (Entry: e, Score: CosineSimilarity(embedding, e.Embedding!)))
                .Where(x => x.Score >= MinimumSimilarity)
                .OrderByDescending(x => x.Score)
                .Take(limit)
                .Select(x => x.Entry);
        }
        else
        {
            candidates = candidates
                .Where(e => e.Subject.Contains(subject, StringComparison.OrdinalIgnoreCase) || e.Content.Contains(subject, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.CreatedAt)
                .Take(limit);
        }

        var results = candidates.ToList();
        foreach (var result in results)
        {
            result.AccessCount++;
        }

        return Task.FromResult<IReadOnlyList<MemoryEntry>>(results);
    }

    public Task<IReadOnlyList<MemoryEntry>> GetRecentAsync(string? sessionId = null, int limit = 50, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entries = _episodic.Values
            .Where(e => sessionId is null || e.SessionId == sessionId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult<IReadOnlyList<MemoryEntry>>(entries);
    }

    public Task<MemoryStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new MemoryStats
        {
            Working = _working.Count,
            Session = _session.Count,
            LongTerm = _longTerm.Count,
            Semantic = _semantic.Count,
            Episodic = _episodic.Count
        });
    }

    internal int WorkingCount => _working.Count;

    internal int SessionCount => _session.Count;

    internal int LongTermCount => _longTerm.Count;

    internal int SemanticCount => _semantic.Count;

    internal int EpisodicCount => _episodic.Count;

    private static double CosineSimilarity(IReadOnlyList<float> a, IReadOnlyList<float> b)
    {
        if (a.Count != b.Count)
        {
            return 0;
        }

        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < a.Count; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        if (normA == 0 || normB == 0)
        {
            return 0;
        }

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
