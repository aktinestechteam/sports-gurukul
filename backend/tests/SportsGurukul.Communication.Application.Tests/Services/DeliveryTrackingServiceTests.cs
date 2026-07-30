using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Services;

public class DeliveryTrackingServiceTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock;
    private readonly Mock<ILogger<DeliveryTrackingService>> _loggerMock;
    private readonly DeliveryTrackingService _service;

    public DeliveryTrackingServiceTests()
    {
        _deliveryRepoMock = new Mock<IDeliveryRepository>();
        _loggerMock = new Mock<ILogger<DeliveryTrackingService>>();
        _service = new DeliveryTrackingService(_deliveryRepoMock.Object, _loggerMock.Object);
    }

    private static NotificationDelivery CreateDelivery(Guid id, Guid notificationId, NotificationStatus status)
    {
        return new NotificationDelivery
        {
            Id = id,
            NotificationId = notificationId,
            Status = status,
            ChannelType = NotificationChannelType.Email,
            AttemptCount = 1,
            Provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "SendGrid", ChannelType = NotificationChannelType.Email, IsActive = true },
        };
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ShouldReturnDeliveries()
    {
        var notificationId = Guid.NewGuid();
        var deliveries = new List<NotificationDelivery>
        {
            CreateDelivery(Guid.NewGuid(), notificationId, NotificationStatus.Delivered),
            CreateDelivery(Guid.NewGuid(), notificationId, NotificationStatus.Failed),
        };
        deliveries[0].SentAt = DateTime.UtcNow.AddMinutes(-10);
        deliveries[0].DeliveredAt = DateTime.UtcNow.AddMinutes(-9);
        deliveries[0].ProviderMessageId = "msg-1";
        deliveries[0].DurationMs = 60000;

        _deliveryRepoMock.Setup(r => r.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveries);

        var result = await _service.GetByNotificationIdAsync(notificationId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.First().NotificationId.Should().Be(notificationId);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateDelivery()
    {
        var deliveryId = Guid.NewGuid();
        var entity = CreateDelivery(deliveryId, Guid.NewGuid(), NotificationStatus.Sending);

        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.UpdateStatusAsync(deliveryId, NotificationStatus.Sent, "prov-msg-1", "OK");

        result.IsSuccess.Should().BeTrue();
        entity.Status.Should().Be(NotificationStatus.Sent);
        entity.ProviderMessageId.Should().Be("prov-msg-1");
        entity.ProviderResponse.Should().Be("OK");
        entity.SentAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldFail_WhenNotFound()
    {
        var deliveryId = Guid.NewGuid();
        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery?)null);

        var result = await _service.UpdateStatusAsync(deliveryId, NotificationStatus.Sent);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task RecordReadAsync_ShouldMarkAsRead()
    {
        var deliveryId = Guid.NewGuid();
        var entity = CreateDelivery(deliveryId, Guid.NewGuid(), NotificationStatus.Delivered);

        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.RecordReadAsync(deliveryId);

        result.IsSuccess.Should().BeTrue();
        entity.Status.Should().Be(NotificationStatus.Read);
        entity.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordReadAsync_ShouldFail_WhenNotFound()
    {
        var deliveryId = Guid.NewGuid();
        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery?)null);

        var result = await _service.RecordReadAsync(deliveryId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task RecordFailureAsync_ShouldRecordFailure()
    {
        var deliveryId = Guid.NewGuid();
        var entity = CreateDelivery(deliveryId, Guid.NewGuid(), NotificationStatus.Sending);

        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.RecordFailureAsync(deliveryId, "Connection timeout", true);

        result.IsSuccess.Should().BeTrue();
        entity.Status.Should().Be(NotificationStatus.Failed);
        entity.FailureReason.Should().Be("Connection timeout");
        entity.AttemptCount.Should().Be(2);
        entity.FailedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordFailureAsync_ShouldFail_WhenNotFound()
    {
        var deliveryId = Guid.NewGuid();
        _deliveryRepoMock.Setup(r => r.GetByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery?)null);

        var result = await _service.RecordFailureAsync(deliveryId, "Error", true);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
