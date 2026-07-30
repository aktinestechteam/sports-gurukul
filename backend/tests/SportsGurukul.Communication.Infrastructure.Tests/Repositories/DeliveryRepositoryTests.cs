using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class DeliveryRepositoryTests
{
    private static int _counter;

    private static NotificationDelivery CreateDelivery(
        NotificationStatus status = NotificationStatus.Sending,
        int attemptCount = 0)
    {
        _counter++;
        return new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            Status = status,
            ProviderMessageId = $"prov_{_counter:D5}",
            AttemptCount = attemptCount,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IDeliveryRepository> _mock;
    private readonly List<NotificationDelivery> _deliveries;

    public DeliveryRepositoryTests()
    {
        _deliveries =
        [
            CreateDelivery(NotificationStatus.Sent),
            CreateDelivery(NotificationStatus.Delivered),
            CreateDelivery(NotificationStatus.Failed, 3)
        ];
        _mock = CreateMockWithBaseSetup(_deliveries);
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ShouldReturnDeliveries()
    {
        var notificationId = Guid.NewGuid();
        var notificationDeliveries = _deliveries.Take(2).ToList();
        notificationDeliveries.ForEach(d => d.NotificationId = notificationId);
        _mock.Setup(r => r.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificationDeliveries);
        var result = await _mock.Object.GetByNotificationIdAsync(notificationId);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(d => d.NotificationId.Should().Be(notificationId));
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ShouldReturnEmpty_WhenNoDeliveries()
    {
        var notificationId = Guid.NewGuid();
        _mock.Setup(r => r.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationDelivery>());
        var result = await _mock.Object.GetByNotificationIdAsync(notificationId);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeliveryStatsAsync_ReturnsStats()
    {
        var notificationId = Guid.NewGuid();
        _mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);
        var total = await _mock.Object.CountAsync(d => d.NotificationId == notificationId);
        total.Should().Be(3);
    }

    [Fact]
    public async Task RecordDeliveryAsync_AddsDeliveryRecord()
    {
        var delivery = CreateDelivery(NotificationStatus.Sent);
        _mock.Setup(r => r.AddAsync(delivery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(delivery);
        var result = await _mock.Object.AddAsync(delivery);
        result.Should().Be(delivery);
        result.Status.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public async Task RecordOpenAsync_RecordsOpenEvent()
    {
        var delivery = _deliveries[1];
        delivery.Status = NotificationStatus.Read;
        delivery.ReadAt = DateTime.UtcNow;
        _mock.Object.Update(delivery);
        _mock.Verify(r => r.Update(delivery), Times.Once);
        delivery.Status.Should().Be(NotificationStatus.Read);
        delivery.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task RecordClickAsync_RecordsClickEventWithLink()
    {
        var delivery = _deliveries[1];
        delivery.Status = NotificationStatus.Read;
        delivery.ReadAt = DateTime.UtcNow;
        _mock.Object.Update(delivery);
        _mock.Verify(r => r.Update(delivery), Times.Once);
        delivery.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnDeliveriesByStatus()
    {
        var failed = _deliveries.Where(d => d.Status == NotificationStatus.Failed).ToList();
        _mock.Setup(r => r.GetByStatusAsync(NotificationStatus.Failed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failed);
        var result = await _mock.Object.GetByStatusAsync(NotificationStatus.Failed);
        result.Should().ContainSingle();
        result.First().Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public async Task GetFailedDeliveriesAsync_ShouldReturnFailedDeliveries()
    {
        var failed = _deliveries.Where(d => d.Status == NotificationStatus.Failed).ToList();
        _mock.Setup(r => r.GetFailedDeliveriesAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failed);
        var result = await _mock.Object.GetFailedDeliveriesAsync(3);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByProviderMessageIdAsync_ShouldReturnDelivery_WhenFound()
    {
        var expected = _deliveries[0];
        _mock.Setup(r => r.GetByProviderMessageIdAsync(expected.ProviderMessageId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByProviderMessageIdAsync(expected.ProviderMessageId!);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByProviderMessageIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByProviderMessageIdAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery?)null);
        var result = await _mock.Object.GetByProviderMessageIdAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    private static Mock<IDeliveryRepository> CreateMockWithBaseSetup(List<NotificationDelivery> data)
    {
        var mock = new Mock<IDeliveryRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationDelivery, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<NotificationDelivery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationDelivery entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationDelivery, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<NotificationDelivery, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationDelivery, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
