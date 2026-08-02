using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface IEpisodicMemoryStore
{
    Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> GetAsync(string sessionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> GetRecentAsync(string? sessionId = null, int limit = 50, CancellationToken cancellationToken = default);
}
