using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Sms;

public class Msg91SmsProvider : ProviderBase, ISmsProvider
{
    public override string Name => "MSG91";
    public override NotificationChannelType ChannelType => NotificationChannelType.SMS;

    public Msg91SmsProvider(ILogger<Msg91SmsProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("MSG91", message));
    }
}
