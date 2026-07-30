using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class NotificationStatisticsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock;
    private readonly NotificationStatisticsQueryHandler _handler;

    public NotificationStatisticsQueryHandlerTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _deliveryRepoMock = new Mock<IDeliveryRepository>();
        _handler = new NotificationStatisticsQueryHandler(_notificationRepoMock.Object, _deliveryRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAggregateStatistics()
    {
        var notifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Delivered, Priority = NotificationPriority.High, Subject = "S1", Body = "B1", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Sent, Priority = NotificationPriority.Normal, Subject = "S2", Body = "B2", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Failed, Priority = NotificationPriority.Low, Subject = "S3", Body = "B3", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Delivered, Priority = NotificationPriority.High, Subject = "S4", Body = "B4", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Queued, Priority = NotificationPriority.Normal, Subject = "S5", Body = "B5", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
        };

        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());

        var result = await _handler.Handle(new NotificationStatisticsQuery(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(5);
        result.Value.Delivered.Should().Be(2);
        result.Value.Sent.Should().Be(1);
        result.Value.Failed.Should().Be(1);
        result.Value.Queued.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldCalculateRatesCorrectly()
    {
        var notifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Failed, Priority = NotificationPriority.High, Subject = "S1", Body = "B1", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Failed, Priority = NotificationPriority.High, Subject = "S2", Body = "B2", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Delivered, Priority = NotificationPriority.High, Subject = "S3", Body = "B3", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Status = NotificationStatus.Delivered, Priority = NotificationPriority.High, Subject = "S4", Body = "B4", CreatedAt = DateTime.UtcNow, ChannelId = Guid.NewGuid() },
        };

        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var deliveries = new List<NotificationDelivery>
        {
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), Status = NotificationStatus.Delivered, DurationMs = 100, ChannelType = NotificationChannelType.Email },
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), Status = NotificationStatus.Delivered, DurationMs = 200, ChannelType = NotificationChannelType.Email },
        };

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveries);

        var result = await _handler.Handle(new NotificationStatisticsQuery(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(4);
        result.Value.Failed.Should().Be(2);
        result.Value.FailureRate.Should().Be(50.0);
        result.Value.AverageDeliveryTimeMs.Should().Be(150);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroValues_WhenNoData()
    {
        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());

        var result = await _handler.Handle(new NotificationStatisticsQuery(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(0);
        result.Value.Queued.Should().Be(0);
        result.Value.Sending.Should().Be(0);
        result.Value.Sent.Should().Be(0);
        result.Value.Delivered.Should().Be(0);
        result.Value.Failed.Should().Be(0);
        result.Value.Cancelled.Should().Be(0);
        result.Value.Expired.Should().Be(0);
        result.Value.Read.Should().Be(0);
        result.Value.AverageDeliveryTimeMs.Should().Be(0);
        result.Value.FailureRate.Should().Be(0);
    }
}
