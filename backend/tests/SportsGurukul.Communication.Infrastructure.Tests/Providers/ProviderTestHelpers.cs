using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers;

public class MockNotificationProvider : INotificationProvider
{
    private readonly Func<ProviderMessage, ProviderSendResult> _sendHandler;
    private readonly Func<bool> _healthHandler;

    public string Name { get; set; } = "MockProvider";
    public NotificationChannelType ChannelType { get; set; } = NotificationChannelType.Email;
    public bool IsAvailable { get; set; } = true;

    public MockNotificationProvider(
        string? name = null,
        NotificationChannelType? channelType = null,
        Func<ProviderMessage, ProviderSendResult>? sendHandler = null,
        Func<bool>? healthHandler = null)
    {
        if (name is not null) Name = name;
        if (channelType.HasValue) ChannelType = channelType.Value;
        _sendHandler = sendHandler ?? (_ => new ProviderSendResult { IsSuccess = true });
        _healthHandler = healthHandler ?? (() => true);
    }

    public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult(_sendHandler(message));

    public Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_healthHandler());
}

public class ProviderResultBuilder
{
    private bool _isSuccess = true;
    private string? _messageId;
    private string? _errorMessage;
    private string? _errorCode;
    private long _durationMs;
    private Dictionary<string, string>? _response;

    public ProviderResultBuilder WithSuccess(bool success)
    {
        _isSuccess = success;
        return this;
    }

    public ProviderResultBuilder WithMessageId(string messageId)
    {
        _messageId = messageId;
        return this;
    }

    public ProviderResultBuilder WithError(string message, string? code = null)
    {
        _isSuccess = false;
        _errorMessage = message;
        _errorCode = code;
        return this;
    }

    public ProviderResultBuilder WithDuration(long ms)
    {
        _durationMs = ms;
        return this;
    }

    public ProviderResultBuilder WithResponse(Dictionary<string, string> response)
    {
        _response = response;
        return this;
    }

    public ProviderSendResult Build()
    {
        return new ProviderSendResult
        {
            IsSuccess = _isSuccess,
            ProviderMessageId = _messageId ?? (_isSuccess ? $"mock_{Guid.NewGuid():N}" : null),
            ErrorMessage = _errorMessage,
            ErrorCode = _errorCode,
            DurationMs = _durationMs > 0 ? _durationMs : Random.Shared.Next(10, 200),
            ProviderResponse = _response ?? (_isSuccess
                ? new Dictionary<string, string> { ["simulated"] = "true", ["provider"] = "Mock" }
                : null)
        };
    }

    public static ProviderResultBuilder Create() => new();
}

public class NotificationMessageBuilder
{
    private string _to = string.Empty;
    private string? _from;
    private string? _subject;
    private string _body = string.Empty;
    private bool _isHtml;
    private string? _contentType;
    private string? _recipientName;
    private Dictionary<string, string>? _headers;
    private List<ProviderAttachment>? _attachments;
    private Dictionary<string, object>? _metadata;

    public NotificationMessageBuilder WithTo(string to)
    {
        _to = to;
        return this;
    }

    public NotificationMessageBuilder WithFrom(string from)
    {
        _from = from;
        return this;
    }

    public NotificationMessageBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public NotificationMessageBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public NotificationMessageBuilder WithHtml(bool isHtml = true)
    {
        _isHtml = isHtml;
        return this;
    }

    public NotificationMessageBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    public NotificationMessageBuilder WithRecipientName(string name)
    {
        _recipientName = name;
        return this;
    }

    public NotificationMessageBuilder WithHeader(string key, string value)
    {
        (_headers ??= new Dictionary<string, string>())[key] = value;
        return this;
    }

    public NotificationMessageBuilder WithAttachment(string fileName, byte[] content, string? contentType = null)
    {
        (_attachments ??= new List<ProviderAttachment>()).Add(new ProviderAttachment
        {
            FileName = fileName,
            Content = content,
            ContentType = contentType ?? "application/octet-stream"
        });
        return this;
    }

    public NotificationMessageBuilder WithMetadata(string key, object value)
    {
        (_metadata ??= new Dictionary<string, object>())[key] = value;
        return this;
    }

    public ProviderMessage Build()
    {
        return new ProviderMessage
        {
            To = _to,
            From = _from,
            Subject = _subject,
            Body = _body,
            IsHtml = _isHtml,
            ContentType = _contentType,
            RecipientName = _recipientName,
            Headers = _headers,
            Attachments = _attachments,
            Metadata = _metadata
        };
    }

    public static NotificationMessageBuilder Create() => new();
}
