using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentRegistry
{
    Task<AgentDefinition> RegisterAsync(AgentDefinition definition, CancellationToken cancellationToken = default);

    Task<AgentDefinition?> GetAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default);
}
