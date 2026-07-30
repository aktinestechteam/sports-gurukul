using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Fixtures;

public static class MockRepositoryFactory
{
    public static Mock<INotificationRepository> CreateNotificationRepository(IReadOnlyList<Notification>? data = null)
    {
        var mock = new Mock<INotificationRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<ITemplateRepository> CreateTemplateRepository(IReadOnlyList<NotificationTemplate>? data = null)
    {
        var mock = new Mock<ITemplateRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IDeliveryRepository> CreateDeliveryRepository(IReadOnlyList<NotificationDelivery>? data = null)
    {
        var mock = new Mock<IDeliveryRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IQueueRepository> CreateQueueRepository(IReadOnlyList<NotificationQueue>? data = null)
    {
        var mock = new Mock<IQueueRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IPreferenceRepository> CreatePreferenceRepository(IReadOnlyList<NotificationPreference>? data = null)
    {
        var mock = new Mock<IPreferenceRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IAuditRepository> CreateAuditRepository(IReadOnlyList<NotificationAudit>? data = null)
    {
        var mock = new Mock<IAuditRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    private static void SetupBaseCalls<TEntity, TRepo>(Mock<TRepo> mock, IReadOnlyList<TEntity> data)
        where TRepo : class, IRepository<TEntity>
        where TEntity : BaseEntity
    {
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));
    }
}

public static class MockServiceFactory
{
    public static Mock<ILogger<T>> CreateLogger<T>() where T : class
    {
        return new Mock<ILogger<T>>();
    }

    public static Mock<IMediator> CreateMediator()
    {
        return new Mock<IMediator>();
    }

    public static Mock<INotificationRepository> CreateNotificationRepoWithData(IReadOnlyList<Notification> data)
    {
        return MockRepositoryFactory.CreateNotificationRepository(data);
    }

    public static Mock<ITemplateRepository> CreateTemplateRepoWithData(IReadOnlyList<NotificationTemplate> data)
    {
        return MockRepositoryFactory.CreateTemplateRepository(data);
    }

    public static Mock<IDeliveryRepository> CreateDeliveryRepoWithData(IReadOnlyList<NotificationDelivery> data)
    {
        return MockRepositoryFactory.CreateDeliveryRepository(data);
    }

    public static Mock<IQueueRepository> CreateQueueRepoWithData(IReadOnlyList<NotificationQueue> data)
    {
        return MockRepositoryFactory.CreateQueueRepository(data);
    }

    public static Mock<IPreferenceRepository> CreatePreferenceRepoWithData(IReadOnlyList<NotificationPreference> data)
    {
        return MockRepositoryFactory.CreatePreferenceRepository(data);
    }

    public static Mock<IAuditRepository> CreateAuditRepoWithData(IReadOnlyList<NotificationAudit> data)
    {
        return MockRepositoryFactory.CreateAuditRepository(data);
    }
}

public static class TestCommandFactory
{
    public static object CreateSendNotificationCommand() => new
    {
        NotificationId = Guid.NewGuid(),
        ChannelType = NotificationChannelType.Email,
        Priority = NotificationPriority.Normal
    };

    public static object CreateScheduleNotificationCommand() => new
    {
        NotificationId = Guid.NewGuid(),
        ScheduledAt = DateTime.UtcNow.AddHours(1)
    };

    public static object CreateCancelNotificationCommand() => new
    {
        NotificationId = Guid.NewGuid()
    };

    public static object CreateUpdateTemplateCommand() => new
    {
        TemplateId = Guid.NewGuid(),
        Name = "Updated Template",
        SubjectTemplate = "Hello {{name}}",
        BodyTemplate = "Updated body {{name}}!"
    };

    public static object CreateSendCampaignCommand() => new
    {
        CampaignId = Guid.NewGuid()
    };

    public static object CreateUpdatePreferenceCommand() => new
    {
        UserId = Guid.NewGuid(),
        ChannelType = NotificationChannelType.Email,
        IsEnabled = true
    };

    public static object CreateRetryDeliveryCommand() => new
    {
        DeliveryId = Guid.NewGuid()
    };
}

public static class TestQueryFactory
{
    public static object CreateGetNotificationQuery() => new
    {
        NotificationId = Guid.NewGuid()
    };

    public static object CreateGetNotificationsByStatusQuery() => new
    {
        Status = NotificationStatus.Sent
    };

    public static object CreateGetNotificationsByUserQuery() => new
    {
        UserId = Guid.NewGuid()
    };

    public static object CreateGetTemplateQuery() => new
    {
        TemplateId = Guid.NewGuid()
    };

    public static object CreateGetTemplatesByChannelQuery() => new
    {
        ChannelType = NotificationChannelType.Email
    };

    public static object CreateGetDeliveryStatusQuery() => new
    {
        NotificationId = Guid.NewGuid()
    };

    public static object CreateGetQueueDepthQuery() => new
    {
        ChannelType = NotificationChannelType.Email
    };

    public static object CreateGetUserPreferencesQuery() => new
    {
        UserId = Guid.NewGuid()
    };

    public static object CreateGetAuditTrailQuery() => new
    {
        EntityType = "Notification",
        EntityId = Guid.NewGuid()
    };
}

public static class DtoFactory
{
    public static Notification CreateNotificationDto() => new()
    {
        Id = Guid.NewGuid(),
        ChannelId = Guid.NewGuid(),
        Subject = "Test Dto Subject",
        Body = "Test Dto Body",
        Priority = NotificationPriority.Normal,
        Status = NotificationStatus.Draft,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationTemplate CreateTemplateDto() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Dto Template",
        ChannelType = NotificationChannelType.Email,
        SubjectTemplate = "Dto {{name}}",
        BodyTemplate = "Dto {{name}}!",
        IsActive = true,
        CurrentVersion = 1,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationDelivery CreateDeliveryDto() => new()
    {
        Id = Guid.NewGuid(),
        NotificationId = Guid.NewGuid(),
        ChannelType = NotificationChannelType.Email,
        Status = NotificationStatus.Sent,
        ProviderMessageId = "dto_msg_001",
        AttemptCount = 1,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationQueue CreateQueueDto() => new()
    {
        Id = Guid.NewGuid(),
        NotificationId = Guid.NewGuid(),
        ChannelType = NotificationChannelType.Email,
        Priority = NotificationPriority.Normal,
        Status = NotificationStatus.Queued,
        QueuedAt = DateTime.UtcNow,
        MaxAttempts = 3,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationPreference CreatePreferenceDto() => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        ChannelType = NotificationChannelType.Email,
        IsEnabled = true,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationAudit CreateAuditDto() => new()
    {
        Id = Guid.NewGuid(),
        EntityType = "Notification",
        EntityId = Guid.NewGuid(),
        Action = "Created",
        ChangedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
    };

    public static NotificationCampaign CreateCampaignDto() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Dto Campaign",
        ChannelType = NotificationChannelType.Email,
        Status = NotificationStatus.Draft,
        CreatedAt = DateTime.UtcNow
    };
}
