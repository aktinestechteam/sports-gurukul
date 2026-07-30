using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class RetryEngineTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepo;
    private readonly CircuitBreaker _circuitBreaker;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<RetryEngine>> _logger;
    private readonly RetryEngine _engine;

    public RetryEngineTests()
    {
        _deliveryRepo = new Mock<IDeliveryRepository>();
        _options = TestDataFactory.CreateOptions(o =>
        {
            o.Retry.MaxRetries = 3;
            o.Retry.BaseDelayMs = 1000;
            o.Retry.MaxDelayMs = 30000;
            o.Retry.BackoffMultiplier = 2.0;
            o.Retry.JitterEnabled = false;
        });
        _logger = new Mock<ILogger<RetryEngine>>();
        _circuitBreaker = new CircuitBreaker(_options, Mock.Of<ILogger<CircuitBreaker>>());
        _engine = new RetryEngine(_deliveryRepo.Object, _circuitBreaker, _options, _logger.Object);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ReturnsSuccess_WhenSendActionSucceeds()
    {
        var deliveryId = Guid.NewGuid();
        var successResult = new DeliveryResultBuilder().Success("msg-123").WithDurationMs(100).Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationDelivery { Id = deliveryId, Retries = new List<NotificationRetry>() });
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        var result = await _engine.ExecuteWithRetryAsync(() => Task.FromResult(successResult), deliveryId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().Be("msg-123");
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_Retries_WhenSendActionFails()
    {
        var deliveryId = Guid.NewGuid();
        var attempts = 0;

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationDelivery { Id = deliveryId, Retries = new List<NotificationRetry>() });
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        var result = await _engine.ExecuteWithRetryAsync(() =>
        {
            attempts++;
            return Task.FromResult(attempts < 3
                ? new DeliveryResultBuilder().Failure("Transient error").Build()
                : new DeliveryResultBuilder().Success("msg-123").Build());
        }, deliveryId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        attempts.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ReturnsFailure_WhenMaxRetriesExceeded()
    {
        var deliveryId = Guid.NewGuid();
        var failureResult = new DeliveryResultBuilder().Failure("Persistent error").Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationDelivery { Id = deliveryId, Retries = new List<NotificationRetry>() });
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        var result = await _engine.ExecuteWithRetryAsync(() => Task.FromResult(failureResult), deliveryId, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("MAX_RETRIES_EXCEEDED");
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ReturnsFailure_WhenCircuitBreakerIsOpen()
    {
        var deliveryId = Guid.NewGuid();
        var failResult = new DeliveryResultBuilder().Failure().Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationDelivery { Id = deliveryId, Retries = new List<NotificationRetry>() });
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        for (int i = 0; i < 5; i++)
            _circuitBreaker.RecordFailure();

        _circuitBreaker.GetState().Should().Be(CircuitState.Open);

        var result = await _engine.ExecuteWithRetryAsync(() => Task.FromResult(failResult), deliveryId, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("CIRCUIT_OPEN");
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_Recovers_AfterCircuitBreakerHalfOpens()
    {
        var deliveryId = Guid.NewGuid();
        var successResult = new DeliveryResultBuilder().Success("msg-456").Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationDelivery { Id = deliveryId, Retries = new List<NotificationRetry>() });
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        for (int i = 0; i < 5; i++)
            _circuitBreaker.RecordFailure();

        _circuitBreaker.GetState().Should().Be(CircuitState.Open);

        var result = await _engine.ExecuteWithRetryAsync(() => Task.FromResult(successResult), deliveryId, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("CIRCUIT_OPEN");
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_IncrementsAttemptCount()
    {
        var deliveryId = Guid.NewGuid();
        var delivery = TestDataFactory.CreateDelivery(status: NotificationStatus.Sending);
        delivery.Retries = new List<NotificationRetry>();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        await _engine.ExecuteWithRetryAsync(
            () => Task.FromResult(new DeliveryResultBuilder().Success().Build()),
            deliveryId, CancellationToken.None);

        delivery.AttemptCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_RecordsRetry_OnEachAttempt()
    {
        var deliveryId = Guid.NewGuid();
        var delivery = TestDataFactory.CreateDelivery(status: NotificationStatus.Sending);
        delivery.Retries = new List<NotificationRetry>();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);
        _deliveryRepo.Setup(r => r.Update(It.IsAny<NotificationDelivery>()));

        await _engine.ExecuteWithRetryAsync(
            () => Task.FromResult(new DeliveryResultBuilder().Success().Build()),
            deliveryId, CancellationToken.None);

        _deliveryRepo.Verify(r => r.Update(It.Is<NotificationDelivery>(d => d.Retries.Count > 0)), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RetryFailedDeliveriesAsync_RequeuesFailedDeliveries()
    {
        var failedDeliveries = new List<NotificationDelivery>
        {
            new NotificationDelivery { Id = Guid.NewGuid(), Status = NotificationStatus.Failed },
            new NotificationDelivery { Id = Guid.NewGuid(), Status = NotificationStatus.Failed }
        };

        _deliveryRepo.Setup(r => r.GetFailedDeliveriesAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedDeliveries);

        await _engine.RetryFailedDeliveriesAsync(CancellationToken.None);

        failedDeliveries.All(d => d.Status == NotificationStatus.Queued).Should().BeTrue();
        _deliveryRepo.Verify(r => r.Update(It.IsAny<NotificationDelivery>()), Times.Exactly(2));
    }
}
