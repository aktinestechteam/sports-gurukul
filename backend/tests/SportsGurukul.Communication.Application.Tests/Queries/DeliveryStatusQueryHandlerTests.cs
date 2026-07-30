using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class DeliveryStatusQueryHandlerTests
{
    private readonly Mock<IDeliveryTrackingService> _deliveryTrackingServiceMock;
    private readonly DeliveryStatusQueryHandler _handler;

    public DeliveryStatusQueryHandlerTests()
    {
        _deliveryTrackingServiceMock = new Mock<IDeliveryTrackingService>();
        _handler = new DeliveryStatusQueryHandler(_deliveryTrackingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDeliveryStatus()
    {
        var notificationId = Guid.NewGuid();
        var deliveries = new List<DeliveryDto>
        {
            new(Guid.NewGuid(), notificationId, Guid.NewGuid(), Guid.NewGuid(), "SendGrid",
                NotificationChannelType.Email, NotificationStatus.Delivered,
                DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, null, null,
                "msg-1", 2, 1200, []),
            new(Guid.NewGuid(), notificationId, Guid.NewGuid(), Guid.NewGuid(), "Twilio",
                NotificationChannelType.SMS, NotificationStatus.Failed,
                DateTime.UtcNow.AddMinutes(-5), null, null, "Network error",
                null, 3, null, [])
        };

        _deliveryTrackingServiceMock.Setup(s => s.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<DeliveryDto>>.Success(deliveries));

        var result = await _handler.Handle(new DeliveryStatusQuery(notificationId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(deliveries);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUnknownNotification()
    {
        var notificationId = Guid.NewGuid();

        _deliveryTrackingServiceMock.Setup(s => s.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<DeliveryDto>>.Failure($"Notification {notificationId} not found"));

        var result = await _handler.Handle(new DeliveryStatusQuery(notificationId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldMapDeliveryEntitiesToDto()
    {
        var notificationId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var deliveries = new List<DeliveryDto>
        {
            new(deliveryId, notificationId, Guid.NewGuid(), Guid.NewGuid(), "SendGrid",
                NotificationChannelType.Email, NotificationStatus.Sent,
                DateTime.UtcNow, null, null, null, "msg-1", 1, 500,
                [new DeliveryRetryDto(Guid.NewGuid(), 1, DateTime.UtcNow, NotificationStatus.Sent, null, false)])
        };

        _deliveryTrackingServiceMock.Setup(s => s.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<DeliveryDto>>.Success(deliveries));

        var result = await _handler.Handle(new DeliveryStatusQuery(notificationId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!.First();
        dto.Id.Should().Be(deliveryId);
        dto.NotificationId.Should().Be(notificationId);
        dto.ChannelType.Should().Be(NotificationChannelType.Email);
        dto.Status.Should().Be(NotificationStatus.Sent);
        dto.AttemptCount.Should().Be(1);
        dto.DurationMs.Should().Be(500);
        dto.Retries.Should().HaveCount(1);
    }
}
