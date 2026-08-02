using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.MultiAgent;

public interface ICollaborationCoordinator
{
    Task<SupervisorRunResult> CoordinateAsync(SupervisorRunRequest request, CancellationToken cancellationToken = default);
}
