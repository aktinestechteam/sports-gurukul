using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class AnalyticsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock;

    public AnalyticsQueryHandlerTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _deliveryRepoMock = new Mock<IDeliveryRepository>();
    }

    private static Notification CreateNotification(Guid id, NotificationStatus status, DateTime createdAt)
    {
        return new Notification
        {
            Id = id,
            Status = status,
            Priority = NotificationPriority.Normal,
            Subject = $"Subject {id}",
            Body = "Body",
            CreatedAt = createdAt,
            ChannelId = Guid.NewGuid(),
            Recipients = new List<NotificationRecipient>()
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnNotificationStatistics()
    {
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now.AddDays(-1)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now.AddDays(-1)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Failed, now.AddDays(-1)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Failed, now.AddDays(-1)),
        };

        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());

        var handler = new NotificationStatisticsQueryHandler(_notificationRepoMock.Object, _deliveryRepoMock.Object);
        var result = await handler.Handle(new NotificationStatisticsQuery(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(4);
        result.Value.Delivered.Should().Be(2);
        result.Value.Failed.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldCalculateChannelBreakdown()
    {
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Failed, now),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now),
        };

        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());

        var handler = new NotificationStatisticsQueryHandler(_notificationRepoMock.Object, _deliveryRepoMock.Object);
        var result = await handler.Handle(new NotificationStatisticsQuery(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(3);
        result.Value.Delivered.Should().Be(2);
        result.Value.Failed.Should().Be(1);
        result.Value.FailureRate.Should().BeApproximately(33.33, 0.01);
    }

    [Fact]
    public async Task Handle_ShouldFilterByDateRange()
    {
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, now.AddDays(-10)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now.AddDays(-3)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Delivered, now.AddDays(-1)),
        };

        _notificationRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _deliveryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());

        var handler = new NotificationStatisticsQueryHandler(_notificationRepoMock.Object, _deliveryRepoMock.Object);
        var result = await handler.Handle(new NotificationStatisticsQuery(now.AddDays(-5), now, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(2);
        result.Value.Delivered.Should().Be(2);
    }
}
