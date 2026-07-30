using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Platform.Communication.Observability;
using SportsGurukul.Platform.Communication.Security;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class DuplicateNotificationTests
{
    private readonly Mock<INotificationRepository> _notifRepoMock = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock = new();
    private readonly Mock<IPreferenceRepository> _prefRepoMock = new();
    private readonly Mock<IRecipientResolver> _recipientResolverMock = new();
    private readonly Mock<ITemplateRenderer> _templateRendererMock = new();
    private readonly Mock<INotificationProviderFactory> _providerFactoryMock = new();
    private readonly RetryEngine _retryEngine;
    private readonly DeliveryTracker _deliveryTracker;
    private readonly DeliveryAuditLogger _auditLogger;
    private readonly DataMasker _dataMasker = new();
    private readonly DeliveryMetricsCollector _metrics;
    private readonly IOptions<CommunicationOptions> _options;

    public DuplicateNotificationTests()
    {
        var cbOptions = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 100,
                OpenDurationSeconds = 60
            }
        });
        var circuitBreaker = new CircuitBreaker(cbOptions, Mock.Of<ILogger<CircuitBreaker>>());
        _retryEngine = new RetryEngine(_deliveryRepoMock.Object, circuitBreaker, cbOptions, Mock.Of<ILogger<RetryEngine>>());
        _deliveryTracker = new DeliveryTracker(_deliveryRepoMock.Object, Mock.Of<ILogger<DeliveryTracker>>());
        _auditLogger = new DeliveryAuditLogger(Mock.Of<IAuditRepository>(), _dataMasker, Mock.Of<ILogger<DeliveryAuditLogger>>());
        _metrics = new DeliveryMetricsCollector(Mock.Of<ILogger<DeliveryMetricsCollector>>());
        _options = Options.Create(new CommunicationOptions());
    }

    [Fact]
    public async Task DispatchAsync_NonExistentNotification_ReturnsFailure()
    {
        _notifRepoMock
            .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var dispatcher = CreateDispatcher();

        var result = await dispatcher.DispatchAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DispatchToRecipientAsync_NonExistentNotification_ReturnsFailure()
    {
        _notifRepoMock
            .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var dispatcher = CreateDispatcher();

        var result = await dispatcher.DispatchToRecipientAsync(Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DispatchToRecipientAsync_NonExistentRecipient_ReturnsFailure()
    {
        var notificationId = Guid.NewGuid();
        var notification = new Notification
        {
            Id = notificationId,
            Recipients = new List<NotificationRecipient>(),
            Channel = new NotificationChannel
            {
                ChannelType = NotificationChannelType.Email
            }
        };

        _notifRepoMock
            .Setup(r => r.GetByIdWithDetailsAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var dispatcher = CreateDispatcher();

        var result = await dispatcher.DispatchToRecipientAsync(notificationId, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DispatchAsync_WithRecipients_UsesProviderFactory()
    {
        var notificationId = Guid.NewGuid();
        var recipients = new List<NotificationRecipient>
        {
            new() { Id = Guid.NewGuid(), DestinationAddress = "a@test.com" },
            new() { Id = Guid.NewGuid(), DestinationAddress = "b@test.com" }
        };
        var notification = new Notification
        {
            Id = notificationId,
            Recipients = recipients,
            Channel = new NotificationChannel
            {
                ChannelType = NotificationChannelType.Email
            }
        };

        _notifRepoMock
            .Setup(r => r.GetByIdWithDetailsAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _prefRepoMock
            .Setup(p => p.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _prefRepoMock
            .Setup(p => p.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        var providerMock = new Mock<INotificationProvider>();
        providerMock
            .Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProviderSendResult { IsSuccess = true });

        _providerFactoryMock
            .Setup(f => f.GetProvider(It.IsAny<NotificationChannelType>()))
            .Returns(providerMock.Object);
        _providerFactoryMock
            .Setup(f => f.GetProvidersForChannel(It.IsAny<NotificationChannelType>()))
            .Returns(new[] { providerMock.Object });

        _templateRendererMock
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string Subject, string Body)>.Success(("Subject", "Body")));

        var dispatcher = CreateDispatcher();

        var result = await dispatcher.DispatchAsync(notificationId);

        result.IsSuccess.Should().BeTrue();
        _providerFactoryMock.Verify(f => f.GetProvidersForChannel(NotificationChannelType.Email), Times.AtLeastOnce);
    }

    private NotificationDispatcher CreateDispatcher()
    {
        return new NotificationDispatcher(
            _notifRepoMock.Object,
            _deliveryRepoMock.Object,
            _prefRepoMock.Object,
            _recipientResolverMock.Object,
            _templateRendererMock.Object,
            _providerFactoryMock.Object,
            _retryEngine,
            _deliveryTracker,
            _auditLogger,
            _dataMasker,
            _metrics,
            _options,
            Mock.Of<ILogger<NotificationDispatcher>>());
    }
}
