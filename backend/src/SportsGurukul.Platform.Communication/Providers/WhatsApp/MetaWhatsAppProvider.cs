using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.WhatsApp;

public class MetaWhatsAppProvider : ProviderBase, IWhatsAppProvider
{
    public override string Name => "MetaWhatsApp";
    public override NotificationChannelType ChannelType => NotificationChannelType.WhatsApp;

    public MetaWhatsAppProvider(ILogger<MetaWhatsAppProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("MetaWhatsApp", message));
    }
}
