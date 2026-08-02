using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Memory;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Memory;

public class MemoryService : IMemoryService
{
    private readonly IWorkingMemoryStore _workingStore;
    private readonly ISessionMemoryStore _sessionStore;
    private readonly ILongTermMemoryStore _longTermStore;
    private readonly ISemanticMemoryStore _semanticStore;
    private readonly IEpisodicMemoryStore _episodicStore;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly ILogger<MemoryService> _logger;

    public MemoryService(
        IWorkingMemoryStore workingStore,
        ISessionMemoryStore sessionStore,
        ILongTermMemoryStore longTermStore,
        ISemanticMemoryStore semanticStore,
        IEpisodicMemoryStore episodicStore,
        IEmbeddingProvider embeddingProvider,
        ILogger<MemoryService>? logger = null)
    {
        _workingStore = workingStore;
        _sessionStore = sessionStore;
        _longTermStore = longTermStore;
        _semanticStore = semanticStore;
        _episodicStore = episodicStore;
        _embeddingProvider = embeddingProvider;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MemoryService>.Instance;
    }

    public IWorkingMemoryStore Working => _workingStore;

    public ISessionMemoryStore Session => _sessionStore;

    public ILongTermMemoryStore LongTerm => _longTermStore;

    public ISemanticMemoryStore Semantic => _semanticStore;

    public IEpisodicMemoryStore Episodic => _episodicStore;

    public async Task WriteAsync(MemoryEntry entry, CancellationToken cancellationToken = default)
    {
        switch (entry.Category)
        {
            case MemoryCategory.Working:
                await _workingStore.AddAsync(entry, cancellationToken);
                break;
            case MemoryCategory.Session:
                await _sessionStore.AddAsync(entry, cancellationToken);
                break;
            case MemoryCategory.Semantic:
                if (entry.Embedding is null || entry.Embedding.Count == 0)
                {
                    entry.Embedding = await _embeddingProvider.EmbedAsync(entry.Content, cancellationToken);
                }

                await _semanticStore.AddAsync(entry, cancellationToken);
                break;
            case MemoryCategory.Episodic:
                await _episodicStore.AddAsync(entry, cancellationToken);
                break;
            default:
                await _longTermStore.AddAsync(entry, cancellationToken);
                break;
        }
    }

    public async Task<IReadOnlyList<MemoryEntry>> RecallAsync(MemoryQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Subject))
        {
            return [];
        }

        var limit = query.Limit > 0 ? query.Limit : 20;
        var results = new List<MemoryEntry>();

        if (query.Category is null or MemoryCategory.Semantic)
        {
            var embedding = await _embeddingProvider.EmbedAsync(query.Subject, cancellationToken);
            var semantic = await _semanticStore.SearchAsync(query.Subject, embedding, limit, cancellationToken);
            results.AddRange(semantic);
        }

        if (query.Category is null or MemoryCategory.LongTerm)
        {
            var longTerm = await _longTermStore.FindAsync(query.Subject, query.TenantId, cancellationToken);
            results.AddRange(longTerm);
        }

        if (query.Category is MemoryCategory.Session)
        {
            var session = await _sessionStore.GetAsync(query.SessionId ?? string.Empty, cancellationToken);
            results.AddRange(session);
        }

        return results
            .Where(r => query.MinImportance is null || r.Importance >= query.MinImportance)
            .Where(r => query.From is null || r.CreatedAt >= query.From)
            .Where(r => query.To is null || r.CreatedAt <= query.To)
            .GroupBy(r => r.Id)
            .Select(g => g.First())
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToList();
    }

    public Task<IReadOnlyList<MemoryEntry>> RecallWorkingAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _workingStore.GetAsync(sessionId, cancellationToken: cancellationToken);

    public Task ClearWorkingAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _workingStore.ClearAsync(sessionId, cancellationToken: cancellationToken);

    public async Task<MemorySnapshot> SnapshotAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var working = await _workingStore.GetAsync(sessionId, cancellationToken: cancellationToken);
        var session = await _sessionStore.GetAsync(sessionId, cancellationToken);
        var recent = await _episodicStore.GetRecentAsync(sessionId, 50, cancellationToken);

        return new MemorySnapshot
        {
            SessionId = sessionId,
            Working = working,
            Session = session,
            Episodic = recent,
            CapturedAt = DateTime.UtcNow
        };
    }

    public Task<MemoryStats> GetStatsAsync(CancellationToken cancellationToken = default) =>
        (_workingStore as InMemoryMemoryStore)?.GetStatsAsync(cancellationToken)
        ?? Task.FromResult(new MemoryStats());

    public Task ClearSessionAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _sessionStore.ClearAsync(sessionId, cancellationToken);
}
