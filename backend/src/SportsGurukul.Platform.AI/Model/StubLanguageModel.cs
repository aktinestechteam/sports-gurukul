using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Model;

public class StubLanguageModel : ILanguageModel
{
    private readonly string _provider;
    private readonly string _model;
    private readonly string? _fixedResponse;
    private readonly ILogger<StubLanguageModel> _logger;

    public StubLanguageModel(string provider, string model, string? fixedResponse = null, ILogger<StubLanguageModel>? logger = null)
    {
        _provider = provider;
        _model = model;
        _fixedResponse = fixedResponse;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubLanguageModel>.Instance;
    }

    public string Provider => _provider;

    public string Model => _model;

    public Task<ModelResponse> GenerateAsync(IReadOnlyList<ModelMessage> messages, ModelOptions? options = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var content = _fixedResponse;
        if (content is null)
        {
            var lastUser = messages.LastOrDefault(m => m.Role == ModelRole.User)?.Content ?? string.Empty;
            content = $"Stub response for: {lastUser}";
        }

        var promptTokens = messages.Sum(m => EstimateTokens(m.Content));
        var completionTokens = EstimateTokens(content);

        return Task.FromResult(new ModelResponse
        {
            Content = content,
            Usage = new ModelUsage { PromptTokens = promptTokens, CompletionTokens = completionTokens },
            FinishReason = "stop",
            Provider = _provider,
            Model = _model
        });
    }

    private static int EstimateTokens(string text) =>
        string.IsNullOrWhiteSpace(text) ? 0 : (int)Math.Ceiling(text.Length / 4.0);
}
