using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Push;

public class ApplePushProvider : ProviderBase, IPushProvider
{
    public override string Name => "ApplePushNotification";
    public override NotificationChannelType ChannelType => NotificationChannelType.PushNotification;

    public ApplePushProvider(ILogger<ApplePushProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("ApplePushNotification", message));
    }
}
