using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers;

public class WebhookProvider : ProviderBase, IWebhookProvider
{
    public override string Name => "WebhookHTTP";
    public override NotificationChannelType ChannelType => NotificationChannelType.Webhook;

    public WebhookProvider(ILogger<WebhookProvider> logger) : base(logger) { }

    public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SimulateSuccess("WebhookHTTP", message));
    }
}
