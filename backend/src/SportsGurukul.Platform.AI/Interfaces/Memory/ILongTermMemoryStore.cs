using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface ILongTermMemoryStore
{
    Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> FindAsync(string subject, string? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> GetAllAsync(CancellationToken cancellationToken = default);
}
