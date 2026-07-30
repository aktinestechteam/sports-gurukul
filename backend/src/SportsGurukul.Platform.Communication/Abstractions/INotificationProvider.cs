using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Abstractions;

public interface INotificationProvider
{
    string Name { get; }
    NotificationChannelType ChannelType { get; }
    bool IsAvailable { get; }
    Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default);
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}

public class ProviderMessage
{
    public string To { get; set; } = string.Empty;
    public string? From { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
    public string? ContentType { get; set; }
    public string? RecipientName { get; set; }
    public IReadOnlyDictionary<string, string>? Headers { get; set; }
    public IReadOnlyList<ProviderAttachment>? Attachments { get; set; }
    public string? ProviderConfigJson { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ProviderAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = "application/octet-stream";
}

public class ProviderSendResult
{
    public bool IsSuccess { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public long DurationMs { get; set; }
    public Dictionary<string, string>? ProviderResponse { get; set; }
}
