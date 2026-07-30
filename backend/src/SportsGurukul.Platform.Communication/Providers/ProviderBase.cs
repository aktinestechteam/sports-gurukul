using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Providers;

public abstract class ProviderBase : INotificationProvider
{
    protected readonly ILogger Logger;

    public abstract string Name { get; }
    public abstract NotificationChannelType ChannelType { get; }
    public virtual bool IsAvailable => true;

    protected ProviderBase(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default);

    public virtual Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    protected ProviderSendResult SimulateSuccess(string providerName, ProviderMessage message)
    {
        Logger.LogInformation(
            "[{Provider}] Simulated send to {To}: {Subject}",
            providerName, message.To, message.Subject);

        return new ProviderSendResult
        {
            IsSuccess = true,
            ProviderMessageId = $"{providerName.ToLowerInvariant()}_{Guid.NewGuid():N}",
            DurationMs = Random.Shared.Next(50, 300),
            ProviderResponse = new Dictionary<string, string>
            {
                ["simulated"] = "true",
                ["provider"] = providerName
            }
        };
    }

    protected ProviderSendResult SimulateFailure(string providerName, string errorMessage)
    {
        Logger.LogWarning(
            "[{Provider}] Simulated failure: {Error}",
            providerName, errorMessage);

        return new ProviderSendResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = "SIMULATED_FAILURE",
            DurationMs = Random.Shared.Next(10, 100)
        };
    }
}
