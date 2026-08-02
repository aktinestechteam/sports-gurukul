using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Planning;

public interface IReflectionService
{
    Task<Reflection> ReflectAsync(ReflectionRequest request, CancellationToken cancellationToken = default);

    Task<SelfEvaluation> EvaluateAsync(SelfEvaluationRequest request, CancellationToken cancellationToken = default);
}
