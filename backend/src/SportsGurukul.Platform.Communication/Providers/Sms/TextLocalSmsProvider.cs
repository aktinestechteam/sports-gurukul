using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Sms;

public class TextLocalSmsProvider : ProviderBase, ISmsProvider
{
    public override string Name => "TextLocal";
    public override NotificationChannelType ChannelType => NotificationChannelType.SMS;

    public TextLocalSmsProvider(ILogger<TextLocalSmsProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("TextLocal", message));
    }
}
