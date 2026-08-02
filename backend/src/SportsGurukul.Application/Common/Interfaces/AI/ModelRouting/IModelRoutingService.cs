using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.ModelRouting;

public interface IModelRoutingService
{
    Task<Result<AIModel>> SelectModelAsync(ModelSelectionRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIModel>>> GetFallbackModelsAsync(Guid modelId, CancellationToken cancellationToken = default);
    Task<Result<AIModel>> GetCostOptimizedModelAsync(AIModelCapability capability, decimal maxCost, CancellationToken cancellationToken = default);
    Task<Result<AIModel>> GetLatencyOptimizedModelAsync(AIModelCapability capability, int maxLatencyMs, CancellationToken cancellationToken = default);
    Task<Result<AIModel>> GetCapabilityModelAsync(AIModelCapability requiredCapability, CancellationToken cancellationToken = default);
}
