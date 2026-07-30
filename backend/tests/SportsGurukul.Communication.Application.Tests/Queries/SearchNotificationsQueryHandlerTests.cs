using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class SearchNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _repositoryMock;
    private readonly SearchNotificationsQueryHandler _handler;

    public SearchNotificationsQueryHandlerTests()
    {
        _repositoryMock = new Mock<INotificationRepository>();
        _handler = new SearchNotificationsQueryHandler(_repositoryMock.Object);
    }

    private static Notification CreateNotification(Guid id, NotificationStatus status, NotificationPriority priority, DateTime createdAt)
    {
        var notification = new Notification
        {
            Id = id,
            Status = status,
            Priority = priority,
            Subject = $"Subject {id}",
            Body = "Body",
            CreatedAt = createdAt,
            ChannelId = Guid.NewGuid(),
            Recipients = [new NotificationRecipient { UserId = Guid.NewGuid(), DestinationAddress = "test@test.com", ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Draft }]
        };
        return notification;
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedResults()
    {
        var notifications = Enumerable.Range(1, 5)
            .Select(i => CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Normal, DateTime.UtcNow.AddHours(-i)))
            .ToList();

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var query = new SearchNotificationsQuery(null, null, null, null, null, null, null, null, null, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(5);
        result.Value.TotalCount.Should().Be(5);
        result.Value.Page.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        var all = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Normal, DateTime.UtcNow),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Failed, NotificationPriority.High, DateTime.UtcNow),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Low, DateTime.UtcNow),
        };

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(all);

        var query = new SearchNotificationsQuery(null, NotificationStatus.Sent, null, null, null, null, null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByDateRange()
    {
        var now = DateTime.UtcNow;
        var all = new List<Notification>
        {
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Normal, now.AddDays(-5)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Normal, now.AddDays(-1)),
            CreateNotification(Guid.NewGuid(), NotificationStatus.Sent, NotificationPriority.Normal, now.AddDays(-10)),
        };

        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(all);

        var query = new SearchNotificationsQuery(null, null, null, null, null, null, null, now.AddDays(-6), now.AddDays(-2), 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoResults()
    {
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var query = new SearchNotificationsQuery("nonexistent", null, null, null, null, null, null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}
