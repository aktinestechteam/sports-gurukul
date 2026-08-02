using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface ISessionMemoryStore
{
    Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> GetAsync(string sessionId, CancellationToken cancellationToken = default);

    Task ClearAsync(string sessionId, CancellationToken cancellationToken = default);
}
