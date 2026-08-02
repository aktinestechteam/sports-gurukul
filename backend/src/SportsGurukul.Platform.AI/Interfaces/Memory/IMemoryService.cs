using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface IMemoryService : IAgentMemory
{
    IWorkingMemoryStore Working { get; }

    ISessionMemoryStore Session { get; }

    ILongTermMemoryStore LongTerm { get; }

    ISemanticMemoryStore Semantic { get; }

    IEpisodicMemoryStore Episodic { get; }

    Task<MemoryStats> GetStatsAsync(CancellationToken cancellationToken = default);

    Task ClearSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
