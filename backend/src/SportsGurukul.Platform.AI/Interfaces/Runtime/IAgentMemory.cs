using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentMemory
{
    Task WriteAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> RecallAsync(MemoryQuery query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> RecallWorkingAsync(string sessionId, CancellationToken cancellationToken = default);

    Task ClearWorkingAsync(string sessionId, CancellationToken cancellationToken = default);

    Task<MemorySnapshot> SnapshotAsync(string sessionId, CancellationToken cancellationToken = default);
}
