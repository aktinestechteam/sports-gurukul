using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.WhatsApp;

public class TwilioWhatsAppProvider : ProviderBase, IWhatsAppProvider
{
    public override string Name => "TwilioWhatsApp";
    public override NotificationChannelType ChannelType => NotificationChannelType.WhatsApp;

    public TwilioWhatsAppProvider(ILogger<TwilioWhatsAppProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("TwilioWhatsApp", message));
    }
}
