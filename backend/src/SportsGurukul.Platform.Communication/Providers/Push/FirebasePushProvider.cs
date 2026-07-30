using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Push;

public class FirebasePushProvider : ProviderBase, IPushProvider
{
    public override string Name => "FirebaseCloudMessaging";
    public override NotificationChannelType ChannelType => NotificationChannelType.PushNotification;

    public FirebasePushProvider(ILogger<FirebasePushProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("FirebaseCloudMessaging", message));
    }
}
