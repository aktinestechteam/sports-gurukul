using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Providers;

public class NotificationProviderFactory : INotificationProviderFactory
{
    private readonly Dictionary<string, INotificationProvider> _providersByName;
    private readonly ILogger<NotificationProviderFactory> _logger;

    public NotificationProviderFactory(
        IEnumerable<INotificationProvider> providers,
        IOptions<CommunicationOptions> options,
        ILogger<NotificationProviderFactory> logger)
    {
        _logger = logger;
        _providersByName = new Dictionary<string, INotificationProvider>(StringComparer.OrdinalIgnoreCase);

        foreach (var provider in providers)
        {
            if (IsProviderEnabled(provider, options.Value))
            {
                _providersByName[provider.Name] = provider;
            }
        }

        _logger.LogInformation("NotificationProviderFactory initialized with {Count} providers: {Providers}",
            _providersByName.Count, string.Join(", ", _providersByName.Keys));
    }

    public INotificationProvider GetProvider(NotificationChannelType channelType)
    {
        var channelProviders = GetProvidersForChannel(channelType);
        if (channelProviders.Count == 0)
        {
            throw new InvalidOperationException($"No active provider found for channel: {channelType}");
        }

        var selected = channelProviders
            .OrderBy(p =>
            {
                var config = GetProviderConfig(p.Name);
                return config?.Priority ?? 100;
            })
            .First();

        _logger.LogDebug("Selected provider {Provider} for channel {Channel}", selected.Name, channelType);
        return selected;
    }

    public IReadOnlyList<INotificationProvider> GetProvidersForChannel(NotificationChannelType channelType)
    {
        return _providersByName.Values
            .Where(p => p.ChannelType == channelType && p.IsAvailable)
            .OrderBy(p =>
            {
                var config = GetProviderConfig(p.Name);
                return config?.Priority ?? 100;
            })
            .ToList();
    }

    public INotificationProvider? GetProviderByName(string providerName)
    {
        _providersByName.TryGetValue(providerName, out var provider);
        return provider;
    }

    public IReadOnlyList<INotificationProvider> GetAllProviders()
    {
        return _providersByName.Values.ToList();
    }

    private bool IsProviderEnabled(INotificationProvider provider, CommunicationOptions options)
    {
        if (!options.Providers.Providers.TryGetValue(provider.Name, out var config))
            return true;

        return config.IsActive;
    }

    private ProviderConfig? GetProviderConfig(string providerName)
    {
        return null;
    }
}
