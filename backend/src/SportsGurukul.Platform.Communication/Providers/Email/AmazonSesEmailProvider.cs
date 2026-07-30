using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Email;

public class AmazonSesEmailProvider : ProviderBase, IEmailProvider
{
    public override string Name => "AmazonSES";
    public override NotificationChannelType ChannelType => NotificationChannelType.Email;

    public AmazonSesEmailProvider(ILogger<AmazonSesEmailProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("AmazonSES", message));
    }
}
