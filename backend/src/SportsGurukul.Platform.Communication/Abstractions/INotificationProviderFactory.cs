using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Abstractions;

public interface INotificationProviderFactory
{
    INotificationProvider GetProvider(NotificationChannelType channelType);
    IReadOnlyList<INotificationProvider> GetProvidersForChannel(NotificationChannelType channelType);
    INotificationProvider? GetProviderByName(string providerName);
    IReadOnlyList<INotificationProvider> GetAllProviders();
}
