using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Model;

public class InMemoryLanguageModelFactory : ILanguageModelFactory
{
    private readonly ConcurrentDictionary<string, Func<ILanguageModel>> _factories = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<InMemoryLanguageModelFactory> _logger;

    public InMemoryLanguageModelFactory(ILogger<InMemoryLanguageModelFactory>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryLanguageModelFactory>.Instance;
        _factories.TryAdd("stub", () => new StubLanguageModel("stub", "stub-model"));
    }

    public IReadOnlyList<string> Providers => _factories.Keys.ToList();

    public void Register(string provider, Func<ILanguageModel> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentNullException.ThrowIfNull(factory);
        _factories[provider] = factory;
        _logger.LogInformation("Registered language model provider '{Provider}'", provider);
    }

    public ILanguageModel? Get(string provider)
    {
        if (_factories.TryGetValue(provider, out var factory))
        {
            return factory();
        }

        return null;
    }

    public ILanguageModel Create(string provider, string model, IDictionary<string, string?>? config = null)
    {
        var resolved = _factories.TryGetValue(provider, out var factory) ? factory() : null;
        if (resolved is not null)
        {
            return resolved;
        }

        _logger.LogWarning("Language model provider '{Provider}' not registered; falling back to stub.", provider);
        return new StubLanguageModel(provider, model);
    }
}
