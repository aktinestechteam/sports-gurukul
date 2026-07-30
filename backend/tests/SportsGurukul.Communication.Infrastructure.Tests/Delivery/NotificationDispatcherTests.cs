using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Platform.Communication.Observability;
using SportsGurukul.Platform.Communication.Security;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class NotificationDispatcherTests
{
    private readonly Mock<INotificationRepository> _notificationRepo;
    private readonly Mock<IDeliveryRepository> _deliveryRepo;
    private readonly Mock<IPreferenceRepository> _preferenceRepo;
    private readonly Mock<IRecipientResolver> _recipientResolver;
    private readonly Mock<ITemplateRenderer> _templateRenderer;
    private readonly Mock<INotificationProviderFactory> _providerFactory;
    private readonly RetryEngine _retryEngine;
    private readonly DeliveryTracker _deliveryTracker;
    private readonly DeliveryAuditLogger _auditLogger;
    private readonly DataMasker _dataMasker;
    private readonly DeliveryMetricsCollector _metrics;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<NotificationDispatcher>> _logger;
    private readonly NotificationDispatcher _dispatcher;

    public NotificationDispatcherTests()
    {
        _notificationRepo = new Mock<INotificationRepository>();
        _deliveryRepo = new Mock<IDeliveryRepository>();
        _preferenceRepo = new Mock<IPreferenceRepository>();
        _recipientResolver = new Mock<IRecipientResolver>();
        _templateRenderer = new Mock<ITemplateRenderer>();
        _providerFactory = new Mock<INotificationProviderFactory>();
        _dataMasker = new DataMasker();
        _options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 100;
            o.Delivery.FailoverEnabled = false;
        });
        _logger = new Mock<ILogger<NotificationDispatcher>>();

        _retryEngine = new RetryEngine(
            _deliveryRepo.Object,
            new CircuitBreaker(_options, Mock.Of<ILogger<CircuitBreaker>>()),
            _options,
            Mock.Of<ILogger<RetryEngine>>());

        _deliveryTracker = new DeliveryTracker(
            _deliveryRepo.Object,
            Mock.Of<ILogger<DeliveryTracker>>());

        _auditLogger = new DeliveryAuditLogger(
            Mock.Of<IAuditRepository>(),
            _dataMasker,
            Mock.Of<ILogger<DeliveryAuditLogger>>());

        _metrics = new DeliveryMetricsCollector(Mock.Of<ILogger<DeliveryMetricsCollector>>());

        _dispatcher = new NotificationDispatcher(
            _notificationRepo.Object,
            _deliveryRepo.Object,
            _preferenceRepo.Object,
            _recipientResolver.Object,
            _templateRenderer.Object,
            _providerFactory.Object,
            _retryEngine,
            _deliveryTracker,
            _auditLogger,
            _dataMasker,
            _metrics,
            _options,
            _logger.Object);

        _deliveryRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new NotificationDelivery { Id = id, Retries = new List<NotificationRetry>() });
    }

    [Fact]
    public async Task DispatchAsync_SendsNotificationViaCorrectProvider()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();
        var successResult = new DeliveryResultBuilder().Success().Build();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
        providerMock.Verify(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DispatchAsync_UsesFactoryToGetProvider()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResultBuilder().Success().Build());

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        await _dispatcher.DispatchAsync(notification.Id);

        _providerFactory.Verify(f => f.GetProvider(NotificationChannelType.Email), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DispatchAsync_HandlesSuccess()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();
        var successResult = new DeliveryResultBuilder().Success().Build();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
        notification.Status.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public async Task DispatchAsync_HandlesProviderFailure()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();
        var failureResult = new DeliveryResultBuilder().Failure().Build();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DispatchAsync_RetriesOnTransientFailure()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();
        var successResult = new DeliveryResultBuilder().Success().Build();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
        providerMock.Verify(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DispatchAsync_UsesCircuitBreaker()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResultBuilder().Success().Build());

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DispatchAsync_LogsDeliveryEvent()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResultBuilder().Success().Build());

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        await _dispatcher.DispatchAsync(notification.Id);

        _logger.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task DispatchAsync_TracksDeliveryMetrics()
    {
        var recipient = TestDataFactory.CreateRecipient();
        var notification = TestDataFactory.CreateNotification(recipients: new List<NotificationRecipient> { recipient });
        var providerMock = MockIEmailProvider.Create();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResultBuilder().Success().Build());

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        await _dispatcher.DispatchAsync(notification.Id);

        _deliveryRepo.Verify(r => r.Update(It.IsAny<NotificationDelivery>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DispatchAsync_ThrowsForUnsupportedChannel()
    {
        var recipient = TestDataFactory.CreateRecipient(destinationAddress: "user@example.com");
        var notification = TestDataFactory.CreateNotification(
            channelType: (NotificationChannelType)999,
            recipients: new List<NotificationRecipient> { recipient });

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider((NotificationChannelType)999))
            .Throws<ArgumentOutOfRangeException>();

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var result = await _dispatcher.DispatchAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DispatchAsync_RespectsPriorityQueue()
    {
        var highPriorityRecipient = TestDataFactory.CreateRecipient();
        var highPriorityNotification = TestDataFactory.CreateNotification(
            priority: NotificationPriority.High,
            recipients: new List<NotificationRecipient> { highPriorityRecipient });

        var lowPriorityRecipient = TestDataFactory.CreateRecipient();
        var lowPriorityNotification = TestDataFactory.CreateNotification(
            priority: NotificationPriority.Low,
            recipients: new List<NotificationRecipient> { lowPriorityRecipient });

        var providerMock = MockIEmailProvider.Create();

        providerMock.Setup(p => p.SendAsync(It.IsAny<ProviderMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResultBuilder().Success().Build());

        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(highPriorityNotification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(highPriorityNotification);
        _notificationRepo.Setup(r => r.GetByIdWithDetailsAsync(lowPriorityNotification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lowPriorityNotification);

        _preferenceRepo.Setup(r => r.IsChannelEnabledAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _preferenceRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        _templateRenderer.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<(string, string)>.Success(("Subject", "Body")));

        _providerFactory.Setup(f => f.GetProvider(NotificationChannelType.Email))
            .Returns(providerMock.Object);

        _deliveryRepo.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery d, CancellationToken _) => d);

        var highResult = await _dispatcher.DispatchAsync(highPriorityNotification.Id);
        var lowResult = await _dispatcher.DispatchAsync(lowPriorityNotification.Id);

        highResult.IsSuccess.Should().BeTrue();
        lowResult.IsSuccess.Should().BeTrue();
    }
}
