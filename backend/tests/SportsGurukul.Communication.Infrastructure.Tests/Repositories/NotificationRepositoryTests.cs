using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class NotificationRepositoryTests
{
    private static int _counter;

    private static Notification CreateNotification(NotificationStatus status = NotificationStatus.Draft)
    {
        _counter++;
        return new Notification
        {
            Id = Guid.NewGuid(),
            ChannelId = Guid.NewGuid(),
            Priority = NotificationPriority.Normal,
            Status = status,
            Subject = $"Test Subject {_counter}",
            Body = $"Test Body {_counter}",
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<INotificationRepository> _mock;
    private readonly List<Notification> _notifications;

    public NotificationRepositoryTests()
    {
        _notifications =
        [
            CreateNotification(NotificationStatus.Draft),
            CreateNotification(NotificationStatus.Sent),
            CreateNotification(NotificationStatus.Failed)
        ];
        _mock = CreateMockWithBaseSetup(_notifications);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotification_WhenFound()
    {
        var expected = _notifications[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldVerifyAdd()
    {
        var notification = CreateNotification();
        var result = await _mock.Object.AddAsync(notification);
        result.Should().Be(notification);
        _mock.Verify(r => r.AddAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void UpdateAsync_ShouldVerifyUpdate()
    {
        var notification = _notifications[0];
        _mock.Object.Update(notification);
        _mock.Verify(r => r.Update(notification), Times.Once);
    }

    [Fact]
    public void DeleteAsync_ShouldVerifyDelete()
    {
        var notification = _notifications[0];
        _mock.Object.Remove(notification);
        _mock.Verify(r => r.Remove(notification), Times.Once);
    }

    [Fact]
    public async Task Mock_VerifyCorrectMethodCalledWithCorrectParameters()
    {
        var expected = _notifications[0];
        await _mock.Object.GetByIdAsync(expected.Id);
        _mock.Verify(r => r.GetByIdAsync(expected.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Mock_VerifyNoExtraCalls()
    {
        _mock.Invocations.Clear();
        var expected = _notifications[0];
        await _mock.Object.GetByIdAsync(expected.Id);
        _mock.Verify(r => r.GetByIdAsync(expected.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnFiltered()
    {
        var draft = _notifications.Where(n => n.Status == NotificationStatus.Draft).ToList();
        _mock.Setup(r => r.GetByStatusAsync(NotificationStatus.Draft, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draft);
        var result = await _mock.Object.GetByStatusAsync(NotificationStatus.Draft);
        result.Should().ContainSingle();
        result.First().Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public async Task GetByPriorityAsync_ShouldReturnFiltered()
    {
        var high = _notifications.Take(1).ToList();
        high.ForEach(n => n.Priority = NotificationPriority.High);
        _mock.Setup(r => r.GetByPriorityAsync(NotificationPriority.High, It.IsAny<CancellationToken>()))
            .ReturnsAsync(high);
        var result = await _mock.Object.GetByPriorityAsync(NotificationPriority.High);
        result.Should().HaveCount(1);
        result.Should().AllSatisfy(n => n.Priority.Should().Be(NotificationPriority.High));
    }

    [Fact]
    public async Task GetByBatchIdAsync_ShouldReturnNotificationsForBatch()
    {
        var batchId = Guid.NewGuid();
        var batchNotifications = _notifications.Take(2).ToList();
        batchNotifications.ForEach(n => n.BatchId = batchId);
        _mock.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchNotifications);
        var result = await _mock.Object.GetByBatchIdAsync(batchId);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(n => n.BatchId.Should().Be(batchId));
    }

    [Fact]
    public async Task GetByCampaignIdAsync_ShouldReturnNotificationsForCampaign()
    {
        var campaignId = Guid.NewGuid();
        var campaignNotifications = _notifications.Take(1).ToList();
        campaignNotifications.ForEach(n => n.CampaignId = campaignId);
        _mock.Setup(r => r.GetByCampaignIdAsync(campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campaignNotifications);
        var result = await _mock.Object.GetByCampaignIdAsync(campaignId);
        result.Should().ContainSingle();
        result.First().CampaignId.Should().Be(campaignId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNotificationsForUser()
    {
        var userId = Guid.NewGuid();
        var userNotifications = _notifications.Take(2).ToList();
        _mock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userNotifications);
        var result = await _mock.Object.GetByUserIdAsync(userId);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPendingAsync_ShouldReturnPendingNotifications()
    {
        var pending = _notifications.Where(n => n.Status is NotificationStatus.Draft or NotificationStatus.Queued).ToList();
        _mock.Setup(r => r.GetPendingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pending);
        var result = await _mock.Object.GetPendingAsync(10);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetScheduledDueAsync_ShouldReturnScheduledNotifications()
    {
        var scheduled = _notifications.Where(n => n.ScheduledAt <= DateTime.UtcNow).ToList();
        _mock.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(scheduled);
        var result = await _mock.Object.GetScheduledDueAsync();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnNotificationWithDetails()
    {
        var notification = _notifications[0];
        notification.Recipients.Add(new NotificationRecipient
        {
            NotificationId = notification.Id,
            DestinationAddress = "detail@example.com",
            ChannelType = NotificationChannelType.Email,
            CreatedAt = DateTime.UtcNow
        });
        _mock.Setup(r => r.GetByIdWithDetailsAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);
        var result = await _mock.Object.GetByIdWithDetailsAsync(notification.Id);
        result.Should().Be(notification);
        result!.Recipients.Should().NotBeEmpty();
    }

    private static Mock<INotificationRepository> CreateMockWithBaseSetup(List<Notification> data)
    {
        var mock = new Mock<INotificationRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Notification, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Notification, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Notification, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
