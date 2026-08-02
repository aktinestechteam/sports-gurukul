using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.MultiAgent;

public interface ISupervisorAgent
{
    Task<SupervisorRunResult> RunAsync(SupervisorRunRequest request, CancellationToken cancellationToken = default);
}
