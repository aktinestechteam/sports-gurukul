using Microsoft.Extensions.Configuration;

namespace SportsGurukul.Platform.Communication.Security;

public class SecretsManager
{
    private readonly IConfiguration _configuration;

    public SecretsManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetSecret(string key)
    {
        return _configuration[$"Communication:Secrets:{key}"]
            ?? _configuration[$"Communication:Providers:{key}:ApiKey"]
            ?? Environment.GetEnvironmentVariable($"COMMUNICATION_{key.ToUpperInvariant().Replace('.', '_')}");
    }

    public string? GetProviderApiKey(string providerName)
    {
        return GetSecret($"{providerName}:ApiKey")
            ?? GetSecret($"{providerName}:ApiSecret")
            ?? _configuration[$"Communication:Providers:{providerName}:Settings:ApiKey"];
    }

    public string? GetProviderSecret(string providerName, string settingName)
    {
        return GetSecret($"{providerName}:{settingName}")
            ?? _configuration[$"Communication:Providers:{providerName}:Settings:{settingName}"];
    }

    public IReadOnlyDictionary<string, string?> GetProviderSecrets(string providerName)
    {
        var section = _configuration.GetSection($"Communication:Providers:{providerName}:Settings");
        return section.GetChildren().ToDictionary(c => c.Key, c => c.Value as string);
    }

    public bool HasSecret(string key)
    {
        return GetSecret(key) is not null;
    }
}
