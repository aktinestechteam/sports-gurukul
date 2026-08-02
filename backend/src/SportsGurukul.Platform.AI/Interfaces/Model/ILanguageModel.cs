using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Model;

public interface ILanguageModel
{
    string Provider { get; }

    string Model { get; }

    Task<ModelResponse> GenerateAsync(IReadOnlyList<ModelMessage> messages, ModelOptions? options = null, CancellationToken cancellationToken = default);
}
