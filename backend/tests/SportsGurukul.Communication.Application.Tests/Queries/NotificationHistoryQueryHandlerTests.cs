using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class NotificationHistoryQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _repositoryMock;
    private readonly NotificationHistoryQueryHandler _handler;

    public NotificationHistoryQueryHandlerTests()
    {
        _repositoryMock = new Mock<INotificationRepository>();
        _handler = new NotificationHistoryQueryHandler(_repositoryMock.Object);
    }

    private static Notification CreateNotification(Guid id, DateTime createdAt, Guid? userId = null)
    {
        return new Notification
        {
            Id = id,
            Status = NotificationStatus.Sent,
            Priority = NotificationPriority.Normal,
            Subject = $"Subject {id}",
            Body = "Body",
            CreatedAt = createdAt,
            ChannelId = Guid.NewGuid(),
            Recipients = userId.HasValue
                ? [new NotificationRecipient { UserId = userId, DestinationAddress = "test@test.com", ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Delivered }]
                : []
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnHistoryForNotification()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), now.AddHours(-2), userId),
            CreateNotification(Guid.NewGuid(), now.AddHours(-1), userId),
            CreateNotification(Guid.NewGuid(), now, userId),
        };

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var query = new NotificationHistoryQuery(userId, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoHistory()
    {
        var userId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var query = new NotificationHistoryQuery(userId, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldOrderByTimestampDescending()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), now, userId),
            CreateNotification(Guid.NewGuid(), now.AddHours(-3), userId),
            CreateNotification(Guid.NewGuid(), now.AddHours(-1), userId),
        };

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var query = new NotificationHistoryQuery(userId, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Select(n => n.Id).Should().ContainInOrder(
            notifications.OrderByDescending(n => n.CreatedAt).Select(n => n.Id));
    }
}
