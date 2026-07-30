using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class DeliveryTrackerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepo;
    private readonly Mock<ILogger<DeliveryTracker>> _logger;
    private readonly DeliveryTracker _tracker;

    public DeliveryTrackerTests()
    {
        _deliveryRepo = new Mock<IDeliveryRepository>();
        _logger = new Mock<ILogger<DeliveryTracker>>();
        _tracker = new DeliveryTracker(_deliveryRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_MarksAsSent_OnSuccess()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Success("ext-msg-id").WithProviderResponse(new Dictionary<string, string> { ["status"] = "ok" }).Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 150, CancellationToken.None);

        delivery.Status.Should().Be(NotificationStatus.Sent);
        delivery.SentAt.Should().NotBeNull();
        delivery.ProviderMessageId.Should().Be("ext-msg-id");
        delivery.DurationMs.Should().Be(150);
        delivery.ProviderResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordDeliveryAttempt_MarksAsFailed_OnFailure()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Failure("Connection timeout").Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 5000, CancellationToken.None);

        delivery.Status.Should().Be(NotificationStatus.Failed);
        delivery.FailedAt.Should().NotBeNull();
        delivery.FailureReason.Should().Be("Connection timeout");
        delivery.DurationMs.Should().Be(5000);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_LogsWarning_WhenDeliveryNotFound()
    {
        var deliveryId = Guid.NewGuid();
        var result = new DeliveryResultBuilder().Success().Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery?)null);

        await _tracker.RecordDeliveryAttempt(deliveryId, result, 100, CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_UpdatesDurationMs()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Success().Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 250, CancellationToken.None);

        delivery.DurationMs.Should().Be(250);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_StoresProviderResponse()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var providerResponse = new Dictionary<string, string> { ["messageId"] = "xyz", ["status"] = "sent" };
        var result = new DeliveryResultBuilder().Success("msg-id").WithProviderResponse(providerResponse).Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 100, CancellationToken.None);

        delivery.ProviderResponse.Should().Contain("messageId");
        delivery.ProviderResponse.Should().Contain("status");
    }

    [Fact]
    public async Task RecordDeliveryAttempt_CallsUpdate_AfterTracking()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Success().Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 100, CancellationToken.None);

        _deliveryRepo.Verify(r => r.Update(delivery), Times.Once);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_LogsSuccess_OnSuccessfulDelivery()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Success("msg-1").Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 100, CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("succeeded")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task RecordDeliveryAttempt_LogsFailure_OnFailedDelivery()
    {
        var delivery = TestDataFactory.CreateDelivery();
        var result = new DeliveryResultBuilder().Failure("Network error").Build();

        _deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);

        await _tracker.RecordDeliveryAttempt(delivery.Id, result, 100, CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
