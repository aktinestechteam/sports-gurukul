using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface ISemanticMemoryStore
{
    Task AddAsync(MemoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MemoryEntry>> SearchAsync(string subject, IReadOnlyList<float>? embedding, int limit, CancellationToken cancellationToken = default);
}
