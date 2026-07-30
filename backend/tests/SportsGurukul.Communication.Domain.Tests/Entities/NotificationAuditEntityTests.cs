using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationAuditEntityTests
{
    [Fact]
    public void CreateAuditEntry_WithAction_ShouldSetPropertiesCorrectly()
    {
        var entityId = Guid.NewGuid();
        var changedBy = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var audit = new NotificationAudit
        {
            Id = Guid.NewGuid(),
            EntityType = "Notification",
            EntityId = entityId,
            Action = "StatusChanged",
            OldValue = "Draft",
            NewValue = "Sent",
            ChangedBy = changedBy,
            ChangedAt = now,
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0",
            CreatedAt = now
        };

        audit.EntityType.Should().Be("Notification");
        audit.EntityId.Should().Be(entityId);
        audit.Action.Should().Be("StatusChanged");
        audit.OldValue.Should().Be("Draft");
        audit.NewValue.Should().Be("Sent");
        audit.ChangedBy.Should().Be(changedBy);
        audit.ChangedAt.Should().Be(now);
        audit.IpAddress.Should().Be("192.168.1.1");
        audit.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void AuditAction_ShouldSupportCreated()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = DateTime.UtcNow
        };

        audit.Action.Should().Be("Created");
    }

    [Fact]
    public void AuditAction_ShouldSupportUpdated()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Updated",
            ChangedAt = DateTime.UtcNow
        };

        audit.Action.Should().Be("Updated");
    }

    [Fact]
    public void AuditAction_ShouldSupportDeleted()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Deleted",
            ChangedAt = DateTime.UtcNow
        };

        audit.Action.Should().Be("Deleted");
    }

    [Fact]
    public void AuditAction_ShouldSupportStatusChange()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "StatusChanged",
            ChangedAt = DateTime.UtcNow
        };

        audit.Action.Should().Be("StatusChanged");
    }

    [Fact]
    public void AuditAction_ShouldSupportSent()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Sent",
            ChangedAt = DateTime.UtcNow
        };

        audit.Action.Should().Be("Sent");
    }

    [Fact]
    public void Timestamp_ShouldStoreChangedAt()
    {
        var now = DateTime.UtcNow;

        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = now
        };

        audit.ChangedAt.Should().Be(now);
    }

    [Fact]
    public void OldAndNewValues_ShouldStoreChanges()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Updated",
            OldValue = "{\"status\":\"Draft\"}",
            NewValue = "{\"status\":\"Sent\"}",
            ChangedAt = DateTime.UtcNow
        };

        audit.OldValue.Should().Be("{\"status\":\"Draft\"}");
        audit.NewValue.Should().Be("{\"status\":\"Sent\"}");
    }

    [Fact]
    public void OldValue_ShouldBeNull_WhenNotSet()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = DateTime.UtcNow
        };

        audit.OldValue.Should().BeNull();
    }

    [Fact]
    public void NewValue_ShouldBeNull_WhenNotSet()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Deleted",
            ChangedAt = DateTime.UtcNow
        };

        audit.NewValue.Should().BeNull();
    }

    [Fact]
    public void ChangedBy_ShouldBeNull_WhenNotSet()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = DateTime.UtcNow
        };

        audit.ChangedBy.Should().BeNull();
    }

    [Fact]
    public void IpAddress_ShouldBeNull_WhenNotSet()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = DateTime.UtcNow
        };

        audit.IpAddress.Should().BeNull();
    }

    [Fact]
    public void UserAgent_ShouldBeNull_WhenNotSet()
    {
        var audit = new NotificationAudit
        {
            EntityType = "Notification",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            ChangedAt = DateTime.UtcNow
        };

        audit.UserAgent.Should().BeNull();
    }

    [Fact]
    public void DefaultChangedAt_ShouldBeMinDate()
    {
        var audit = new NotificationAudit();

        audit.ChangedAt.Should().Be(default);
    }
}
