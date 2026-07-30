using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Sms;

public class TwilioSmsProvider : ProviderBase, ISmsProvider
{
    public override string Name => "TwilioSMS";
    public override NotificationChannelType ChannelType => NotificationChannelType.SMS;

    public TwilioSmsProvider(ILogger<TwilioSmsProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("TwilioSMS", message));
    }
}
