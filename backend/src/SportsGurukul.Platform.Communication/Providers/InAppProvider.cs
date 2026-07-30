using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers;

public class InAppProvider : ProviderBase, IInAppProvider
{
    public override string Name => "SignalRInApp";
    public override NotificationChannelType ChannelType => NotificationChannelType.InAppNotification;

    public InAppProvider(ILogger<InAppProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("SignalRInApp", message));
    }
}
