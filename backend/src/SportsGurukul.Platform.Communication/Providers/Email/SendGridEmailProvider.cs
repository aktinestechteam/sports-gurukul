using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Email;

public class SendGridEmailProvider : ProviderBase, IEmailProvider
{
    public override string Name => "SendGrid";
    public override NotificationChannelType ChannelType => NotificationChannelType.Email;

    public SendGridEmailProvider(ILogger<SendGridEmailProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("SendGrid", message));
    }
}
