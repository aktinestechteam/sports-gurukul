using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Email;

public class SmtpEmailProvider : ProviderBase, IEmailProvider
{
    public override string Name => "SMTP";
    public override NotificationChannelType ChannelType => NotificationChannelType.Email;

    public SmtpEmailProvider(ILogger<SmtpEmailProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("SMTP", message));
    }
}
