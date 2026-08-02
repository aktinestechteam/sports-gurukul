using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface IToolRegistry
{
    Task<ITool> RegisterAsync(ITool tool, CancellationToken cancellationToken = default);

    Task<ITool?> GetAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ITool>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ITool>> GetByTypeAsync(ToolType type, CancellationToken cancellationToken = default);

    Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default);
}
