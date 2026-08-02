using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface IWorkingMemoryStore
{
    Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> GetAsync(string sessionId, string? tenantId = null, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid entryId, CancellationToken cancellationToken = default);

    Task ClearAsync(string sessionId, string? tenantId = null, CancellationToken cancellationToken = default);
}
