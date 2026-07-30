using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers.Email;

public class AzureCommunicationEmailProvider : ProviderBase, IEmailProvider
{
    public override string Name => "AzureCommunicationServices";
    public override NotificationChannelType ChannelType => NotificationChannelType.Email;

    public AzureCommunicationEmailProvider(ILogger<AzureCommunicationEmailProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("AzureCommunicationServices", message));
    }
}
