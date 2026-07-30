using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq.Protected;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using System.Net;

namespace SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

public static class MockIEmailProvider
{
    public static Mock<IEmailProvider> Create(string name = "TestEmailProvider", bool isAvailable = true)
    {
        var mock = new Mock<IEmailProvider>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.ChannelType).Returns(NotificationChannelType.Email);
        mock.Setup(p => p.IsAvailable).Returns(isAvailable);
        return mock;
    }
}

public static class MockISmsProvider
{
    public static Mock<ISmsProvider> Create(string name = "TestSmsProvider", bool isAvailable = true)
    {
        var mock = new Mock<ISmsProvider>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.ChannelType).Returns(NotificationChannelType.SMS);
        mock.Setup(p => p.IsAvailable).Returns(isAvailable);
        return mock;
    }
}

public static class MockIPushProvider
{
    public static Mock<IPushProvider> Create(string name = "TestPushProvider", bool isAvailable = true)
    {
        var mock = new Mock<IPushProvider>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.ChannelType).Returns(NotificationChannelType.PushNotification);
        mock.Setup(p => p.IsAvailable).Returns(isAvailable);
        return mock;
    }
}

public static class MockIWhatsAppProvider
{
    public static Mock<IWhatsAppProvider> Create(string name = "TestWhatsAppProvider", bool isAvailable = true)
    {
        var mock = new Mock<IWhatsAppProvider>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.ChannelType).Returns(NotificationChannelType.WhatsApp);
        mock.Setup(p => p.IsAvailable).Returns(isAvailable);
        return mock;
    }
}

public static class MockIWebhookProvider
{
    public static Mock<IWebhookProvider> Create(string name = "TestWebhookProvider", bool isAvailable = true)
    {
        var mock = new Mock<IWebhookProvider>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.ChannelType).Returns(NotificationChannelType.Webhook);
        mock.Setup(p => p.IsAvailable).Returns(isAvailable);
        return mock;
    }
}

public class NotificationMessageBuilder
{
    private readonly ProviderMessage _message = new();

    public NotificationMessageBuilder WithTo(string to)
    {
        _message.To = to;
        return this;
    }

    public NotificationMessageBuilder WithSubject(string subject)
    {
        _message.Subject = subject;
        return this;
    }

    public NotificationMessageBuilder WithBody(string body)
    {
        _message.Body = body;
        return this;
    }

    public NotificationMessageBuilder WithRecipientName(string name)
    {
        _message.RecipientName = name;
        return this;
    }

    public NotificationMessageBuilder AsHtml()
    {
        _message.IsHtml = true;
        return this;
    }

    public NotificationMessageBuilder WithMetadata(string key, object value)
    {
        _message.Metadata ??= new Dictionary<string, object>();
        _message.Metadata[key] = value;
        return this;
    }

    public ProviderMessage Build() => _message;
}

public class DeliveryResultBuilder
{
    private readonly ProviderSendResult _result = new();

    public DeliveryResultBuilder Success(string? providerMessageId = null)
    {
        _result.IsSuccess = true;
        _result.ProviderMessageId = providerMessageId ?? Guid.NewGuid().ToString("N");
        return this;
    }

    public DeliveryResultBuilder Failure(string errorMessage = "Delivery failed", string errorCode = "DELIVERY_ERROR")
    {
        _result.IsSuccess = false;
        _result.ErrorMessage = errorMessage;
        _result.ErrorCode = errorCode;
        return this;
    }

    public DeliveryResultBuilder WithDurationMs(long durationMs)
    {
        _result.DurationMs = durationMs;
        return this;
    }

    public DeliveryResultBuilder WithProviderResponse(Dictionary<string, string> response)
    {
        _result.ProviderResponse = response;
        return this;
    }

    public ProviderSendResult Build() => _result;
}

public class MockHttpMessageHandlerBuilder
{
    private readonly Mock<HttpMessageHandler> _handlerMock = new(MockBehavior.Strict);

    public MockHttpMessageHandlerBuilder RespondWith(HttpStatusCode statusCode, string? responseBody = null)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = responseBody is not null ? new StringContent(responseBody) : null
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return this;
    }

    public MockHttpMessageHandlerBuilder RespondWithSequence(params (HttpStatusCode StatusCode, string? Body)[] responses)
    {
        var queue = new Queue<(HttpStatusCode, string?)>(responses);
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() =>
            {
                var (status, body) = queue.Dequeue();
                return Task.FromResult(new HttpResponseMessage(status)
                {
                    Content = body is not null ? new StringContent(body) : null
                });
            });

        return this;
    }

    public MockHttpMessageHandlerBuilder ThrowsOnSend(Exception exception)
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        return this;
    }

    public HttpClient CreateClient()
    {
        return new HttpClient(_handlerMock.Object);
    }

    public Mock<HttpMessageHandler> HandlerMock => _handlerMock;
}

public static class TestDataFactory
{
    public static NotificationRecipient CreateRecipient(
        Guid? userId = null,
        string destinationAddress = "user@example.com",
        string? recipientName = "Test User")
    {
        return new NotificationRecipient
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DestinationAddress = destinationAddress,
            RecipientName = recipientName,
            ChannelType = NotificationChannelType.Email
        };
    }

    public static Domain.Entities.Notification.Notification CreateNotification(
        NotificationPriority priority = NotificationPriority.Normal,
        NotificationChannelType channelType = NotificationChannelType.Email,
        List<NotificationRecipient>? recipients = null)
    {
        var channel = new NotificationChannel
        {
            Id = Guid.NewGuid(),
            ChannelType = channelType,
            Code = channelType.ToString(),
            Name = channelType.ToString()
        };

        return new Domain.Entities.Notification.Notification
        {
            Id = Guid.NewGuid(),
            Priority = priority,
            Status = NotificationStatus.Queued,
            Subject = "Test Subject",
            Body = "Test Body",
            Channel = channel,
            ChannelId = channel.Id,
            Recipients = recipients ?? new List<NotificationRecipient> { CreateRecipient() }
        };
    }

    public static NotificationQueue CreateQueueItem(
        Guid? notificationId = null,
        NotificationPriority priority = NotificationPriority.Normal,
        NotificationStatus status = NotificationStatus.Queued)
    {
        return new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId ?? Guid.NewGuid(),
            Priority = priority,
            Status = status,
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow,
            MaxAttempts = 3
        };
    }

    public static NotificationDelivery CreateDelivery(
        Guid? notificationId = null,
        NotificationStatus status = NotificationStatus.Sending)
    {
        return new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId ?? Guid.NewGuid(),
            Status = status,
            ChannelType = NotificationChannelType.Email,
            AttemptCount = 0,
            Retries = new List<NotificationRetry>()
        };
    }

    public static IOptions<CommunicationOptions> CreateOptions(Action<CommunicationOptions>? configure = null)
    {
        var options = new CommunicationOptions();
        configure?.Invoke(options);
        return Microsoft.Extensions.Options.Options.Create(options);
    }
}
