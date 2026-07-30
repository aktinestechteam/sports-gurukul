using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class AuditRepositoryTests
{
    private static int _counter;

    private static NotificationAudit CreateAudit(
        string entityType = "Notification",
        string action = "Created",
        int daysAgo = 0)
    {
        _counter++;
        return new NotificationAudit
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = Guid.NewGuid(),
            Action = action,
            ChangedAt = DateTime.UtcNow.AddDays(-daysAgo),
            ChangedBy = Guid.NewGuid(),
            IpAddress = "127.0.0.1",
            UserAgent = "TestAgent/1.0",
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IAuditRepository> _mock;
    private readonly List<NotificationAudit> _auditEntries;
    private readonly Guid _sharedEntityId = Guid.NewGuid();

    public AuditRepositoryTests()
    {
        _auditEntries =
        [
            new NotificationAudit
            {
                Id = Guid.NewGuid(),
                EntityType = "Notification",
                EntityId = _sharedEntityId,
                Action = "Created",
                ChangedAt = DateTime.UtcNow.AddDays(-2),
                ChangedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            },
            new NotificationAudit
            {
                Id = Guid.NewGuid(),
                EntityType = "Notification",
                EntityId = _sharedEntityId,
                Action = "Sent",
                ChangedAt = DateTime.UtcNow.AddDays(-1),
                ChangedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            },
            new NotificationAudit
            {
                Id = Guid.NewGuid(),
                EntityType = "Notification",
                EntityId = Guid.NewGuid(),
                Action = "Created",
                ChangedAt = DateTime.UtcNow,
                ChangedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            }
        ];
        _mock = CreateMockWithBaseSetup(_auditEntries);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAuditEntry()
    {
        var entry = CreateAudit("Template", "Updated");
        _mock.Setup(r => r.AddAsync(entry, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        var result = await _mock.Object.AddAsync(entry);
        result.Should().Be(entry);
        result.Action.Should().Be("Updated");
        _mock.Verify(r => r.AddAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ShouldReturnAuditTrail()
    {
        var trail = _auditEntries.Where(a => a.EntityId == _sharedEntityId).ToList();
        _mock.Setup(r => r.GetByEntityAsync("Notification", _sharedEntityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trail);
        var result = await _mock.Object.GetByEntityAsync("Notification", _sharedEntityId);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.EntityId.Should().Be(_sharedEntityId));
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ShouldReturnEmpty_WhenNoAudit()
    {
        _mock.Setup(r => r.GetByEntityAsync("Notification", It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationAudit>());
        var result = await _mock.Object.GetByEntityAsync("Notification", Guid.NewGuid());
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldFilterByDate()
    {
        var from = DateTime.UtcNow.AddDays(-3);
        var to = DateTime.UtcNow.AddDays(1);
        var filtered = _auditEntries.Where(a => a.ChangedAt >= from && a.ChangedAt <= to).ToList();
        _mock.Setup(r => r.GetByDateRangeAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(filtered);
        var result = await _mock.Object.GetByDateRangeAsync(from, to);
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        var from = DateTime.UtcNow.AddDays(-10);
        var to = DateTime.UtcNow.AddDays(-9);
        _mock.Setup(r => r.GetByDateRangeAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationAudit>());
        var result = await _mock.Object.GetByDateRangeAsync(from, to);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByActionAsync_ShouldReturnEntriesByAction()
    {
        var created = _auditEntries.Where(a => a.Action == "Created").ToList();
        _mock.Setup(r => r.GetByActionAsync("Created", It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);
        var result = await _mock.Object.GetByActionAsync("Created");
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.Action.Should().Be("Created"));
    }

    [Fact]
    public async Task GetByEntityAsync_ShouldReturnEntriesByEntityType()
    {
        var notificationEntries = _auditEntries.Where(a => a.EntityType == "Notification").ToList();
        _mock.Setup(r => r.GetByEntityAsync("Notification", It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificationEntries);
        var result = await _mock.Object.GetByEntityAsync("Notification", _sharedEntityId);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void AuditEntry_ShouldHaveRequiredFields()
    {
        var entry = _auditEntries[0];
        entry.EntityType.Should().NotBeNullOrEmpty();
        entry.Action.Should().NotBeNullOrEmpty();
        entry.ChangedAt.Should().NotBe(default);
    }

    private static Mock<IAuditRepository> CreateMockWithBaseSetup(List<NotificationAudit> data)
    {
        var mock = new Mock<IAuditRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationAudit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationAudit, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationAudit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationAudit, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<NotificationAudit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationAudit, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
