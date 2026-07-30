using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SportsGurukul.Api;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests;

public class CommunicationTestApplicationFactory : WebApplicationFactory<ApiMarker>
{
    private readonly List<Domain.Entities.Notification.Notification> _notifications = new();
    private readonly List<NotificationTemplate> _templates = new();
    private readonly List<NotificationPreference> _preferences = new();
    private readonly List<NotificationDelivery> _deliveries = new();
    private readonly List<NotificationQueue> _queueItems = new();
    private readonly List<NotificationAudit> _auditLogs = new();
    private readonly List<NotificationCampaign> _campaigns = new();
    private readonly List<NotificationChannel> _channels = new();
    private readonly List<NotificationProvider> _providers = new();
    private readonly List<NotificationBatch> _batches = new();
    private readonly List<NotificationSchedule> _schedules = new();
    private readonly List<NotificationRetry> _retries = new();
    private readonly List<NotificationAttachment> _attachments = new();
    private readonly List<NotificationRecipient> _recipients = new();
    private readonly List<NotificationSubscription> _subscriptions = new();
    private readonly List<NotificationEvent> _events = new();
    private readonly List<TemplateVersion> _templateVersions = new();
    private readonly List<TemplateVariable> _templateVariables = new();

    public CommunicationTestApplicationFactory()
    {
        _channels.Add(new NotificationChannel
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Name = "Email",
            Code = "EMAIL",
            ChannelType = NotificationChannelType.Email,
            IsActive = true,
            SortOrder = 1,
            CreatedAt = DateTime.UtcNow
        });
        _channels.Add(new NotificationChannel
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Name = "SMS",
            Code = "SMS",
            ChannelType = NotificationChannelType.SMS,
            IsActive = true,
            SortOrder = 2,
            CreatedAt = DateTime.UtcNow
        });

        _providers.Add(new NotificationProvider
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Name = "Test SMTP",
            ChannelType = NotificationChannelType.Email,
            ChannelId = _channels[0].Id,
            IsActive = true,
            IsDefault = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        });
    }

    public const string TestJwtSigningKey = "Integration-Test-Secret-Key-At-Least-32-Characters-Long!!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "SportsGurukul",
                    ValidAudience = "SportsGurukul",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services.PostConfigure<JwtOptions>(options =>
            {
                options.Issuer = "SportsGurukul";
                options.Audience = "SportsGurukul";
                options.SigningKey = TestJwtSigningKey;
                options.AccessTokenExpirationMinutes = 60;
            });

            var toRemove = services.Where(s =>
                s.ServiceType == typeof(INotificationRepository) ||
                s.ServiceType == typeof(ITemplateRepository) ||
                s.ServiceType == typeof(IPreferenceRepository) ||
                s.ServiceType == typeof(IDeliveryRepository) ||
                s.ServiceType == typeof(IQueueRepository) ||
                s.ServiceType == typeof(IAuditRepository) ||
                s.ServiceType == typeof(IRepository<NotificationTemplate>) ||
                s.ServiceType == typeof(IRepository<NotificationPreference>) ||
                s.ServiceType == typeof(IRepository<NotificationDelivery>) ||
                s.ServiceType == typeof(IRepository<NotificationQueue>) ||
                s.ServiceType == typeof(IRepository<NotificationAudit>) ||
                s.ServiceType == typeof(IRepository<NotificationCampaign>) ||
                s.ServiceType == typeof(IRepository<NotificationChannel>) ||
                s.ServiceType == typeof(IRepository<NotificationProvider>) ||
                s.ServiceType == typeof(IRepository<NotificationBatch>) ||
                s.ServiceType == typeof(IRepository<NotificationSchedule>) ||
                s.ServiceType == typeof(IRepository<NotificationRetry>) ||
                s.ServiceType == typeof(IRepository<NotificationAttachment>) ||
                s.ServiceType == typeof(IRepository<NotificationRecipient>) ||
                s.ServiceType == typeof(IRepository<NotificationSubscription>) ||
                s.ServiceType == typeof(IRepository<NotificationEvent>) ||
                s.ServiceType == typeof(IRepository<TemplateVersion>) ||
                s.ServiceType == typeof(IRepository<TemplateVariable>) ||
                s.ServiceType == typeof(INotificationDispatcher) ||
                s.ServiceType == typeof(IQueueService) ||
                s.ServiceType == typeof(IRecipientResolver) ||
                s.ServiceType == typeof(ITemplateRenderer) ||
                s.ServiceType == typeof(IBusinessRuleValidator)).ToList();

            foreach (var s in toRemove) services.Remove(s);

            services.AddSingleton<MockNotificationDispatcher>();
            services.AddSingleton<INotificationDispatcher>(sp => sp.GetRequiredService<MockNotificationDispatcher>());
            services.AddSingleton<MockQueueService>();
            services.AddSingleton<IQueueService>(sp => sp.GetRequiredService<MockQueueService>());
            services.AddSingleton<MockRecipientResolver>();
            services.AddSingleton<IRecipientResolver>(sp => sp.GetRequiredService<MockRecipientResolver>());
            services.AddSingleton<MockTemplateRenderer>();
            services.AddSingleton<ITemplateRenderer>(sp => sp.GetRequiredService<MockTemplateRenderer>());
            services.AddSingleton<MockBusinessRuleValidator>();
            services.AddSingleton<IBusinessRuleValidator>(sp => sp.GetRequiredService<MockBusinessRuleValidator>());

            RegisterRepositories(services);
        });
    }

    private void RegisterRepositories(IServiceCollection services)
    {
        services.AddSingleton(_ => new InMemoryNotificationRepository(_notifications, _recipients, _attachments));
        services.AddSingleton<INotificationRepository>(sp => sp.GetRequiredService<InMemoryNotificationRepository>());
        services.AddSingleton<IRepository<Domain.Entities.Notification.Notification>>(sp => sp.GetRequiredService<InMemoryNotificationRepository>());

        services.AddSingleton(_ => new InMemoryTemplateRepository(_templates, _templateVersions, _templateVariables));
        services.AddSingleton<ITemplateRepository>(sp => sp.GetRequiredService<InMemoryTemplateRepository>());
        services.AddSingleton<IRepository<NotificationTemplate>>(sp => sp.GetRequiredService<InMemoryTemplateRepository>());

        services.AddSingleton(_ => new InMemoryPreferenceRepository(_preferences));
        services.AddSingleton<IPreferenceRepository>(sp => sp.GetRequiredService<InMemoryPreferenceRepository>());
        services.AddSingleton<IRepository<NotificationPreference>>(sp => sp.GetRequiredService<InMemoryPreferenceRepository>());

        services.AddSingleton(_ => new InMemoryDeliveryRepository(_deliveries, _retries));
        services.AddSingleton<IDeliveryRepository>(sp => sp.GetRequiredService<InMemoryDeliveryRepository>());
        services.AddSingleton<IRepository<NotificationDelivery>>(sp => sp.GetRequiredService<InMemoryDeliveryRepository>());

        services.AddSingleton(_ => new InMemoryQueueRepository(_queueItems));
        services.AddSingleton<IQueueRepository>(sp => sp.GetRequiredService<InMemoryQueueRepository>());
        services.AddSingleton<IRepository<NotificationQueue>>(sp => sp.GetRequiredService<InMemoryQueueRepository>());

        services.AddSingleton(_ => new InMemoryAuditRepository(_auditLogs));
        services.AddSingleton<IAuditRepository>(sp => sp.GetRequiredService<InMemoryAuditRepository>());
        services.AddSingleton<IRepository<NotificationAudit>>(sp => sp.GetRequiredService<InMemoryAuditRepository>());

        services.AddSingleton(_ => new InMemoryCampaignRepository(_campaigns));
        services.AddSingleton<IRepository<NotificationCampaign>>(sp => sp.GetRequiredService<InMemoryCampaignRepository>());
    }

    public InMemoryNotificationRepository Notifications =>
        Services.GetRequiredService<InMemoryNotificationRepository>();
    public InMemoryTemplateRepository Templates =>
        Services.GetRequiredService<InMemoryTemplateRepository>();
    public InMemoryPreferenceRepository Preferences =>
        Services.GetRequiredService<InMemoryPreferenceRepository>();
    public InMemoryDeliveryRepository Deliveries =>
        Services.GetRequiredService<InMemoryDeliveryRepository>();
    public InMemoryQueueRepository Queue =>
        Services.GetRequiredService<InMemoryQueueRepository>();
    public InMemoryAuditRepository Audits =>
        Services.GetRequiredService<InMemoryAuditRepository>();
    public MockNotificationDispatcher Dispatcher =>
        Services.GetRequiredService<MockNotificationDispatcher>();
    public MockQueueService QueueService =>
        Services.GetRequiredService<MockQueueService>();
    public MockRecipientResolver RecipientResolver =>
        Services.GetRequiredService<MockRecipientResolver>();
    public MockTemplateRenderer TemplateRenderer =>
        Services.GetRequiredService<MockTemplateRenderer>();
    public MockBusinessRuleValidator RuleValidator =>
        Services.GetRequiredService<MockBusinessRuleValidator>();
}

#region In-Memory Repositories

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly List<Domain.Entities.Notification.Notification> _entities;
    private readonly List<NotificationRecipient> _recipients;
    private readonly List<NotificationAttachment> _attachments;

    public IReadOnlyList<Domain.Entities.Notification.Notification> Items => _entities.AsReadOnly();

    public InMemoryNotificationRepository(
        List<Domain.Entities.Notification.Notification> entities,
        List<NotificationRecipient> recipients,
        List<NotificationAttachment> attachments)
    {
        _entities = entities;
        _recipients = recipients;
        _attachments = attachments;
    }

    public Task<Domain.Entities.Notification.Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<Domain.Entities.Notification.Notification?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted);
        if (entity is not null)
        {
            entity.Recipients = _recipients.Where(r => r.NotificationId == id).ToList();
            entity.Attachments = _attachments.Where(a => a.NotificationId == id).ToList();
        }
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByStatusAsync(NotificationStatus status, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.Status == status && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByPriorityAsync(NotificationPriority priority, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.Priority == priority && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByBatchIdAsync(Guid batchId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.BatchId == batchId && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByCampaignIdAsync(Guid campaignId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.CampaignId == campaignId && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetPendingAsync(int take, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.Status == NotificationStatus.Queued && !e.IsDeleted).Take(take).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetScheduledDueAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => e.Status == NotificationStatus.Scheduled && e.ScheduledAt <= DateTime.UtcNow && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<Domain.Entities.Notification.Notification>> FindAsync(Expression<Func<Domain.Entities.Notification.Notification, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Domain.Entities.Notification.Notification>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<Domain.Entities.Notification.Notification> AddAsync(Domain.Entities.Notification.Notification entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(Domain.Entities.Notification.Notification entity) { }

    public void Remove(Domain.Entities.Notification.Notification entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<Domain.Entities.Notification.Notification, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<Domain.Entities.Notification.Notification, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryTemplateRepository : ITemplateRepository
{
    private readonly List<NotificationTemplate> _entities;
    private readonly List<TemplateVersion> _versions;
    private readonly List<TemplateVariable> _variables;

    public IReadOnlyList<NotificationTemplate> Items => _entities.AsReadOnly();

    public InMemoryTemplateRepository(
        List<NotificationTemplate> entities,
        List<TemplateVersion> versions,
        List<TemplateVariable> variables)
    {
        _entities = entities;
        _versions = versions;
        _variables = variables;
    }

    public Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e =>
            e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted));

    public Task<NotificationTemplate?> GetWithVersionsAsync(Guid id, CancellationToken ct = default)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted);
        if (entity is not null)
        {
            entity.Versions = _versions.Where(v => v.TemplateId == id).ToList();
            entity.Variables = _variables.Where(v => v.TemplateId == id).ToList();
        }
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<NotificationTemplate>> GetByChannelAsync(NotificationChannelType channelType, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationTemplate>>(
            _entities.Where(e => e.ChannelType == channelType && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationTemplate>>(
            _entities.Where(e => e.IsActive && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationTemplate>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationTemplate>> FindAsync(Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationTemplate>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationTemplate> AddAsync(NotificationTemplate entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        if (entity.Versions is not null)
        {
            foreach (var v in entity.Versions)
            {
                v.TemplateId = entity.Id;
                _versions.Add(v);
            }
        }
        if (entity.Variables is not null)
        {
            foreach (var v in entity.Variables)
            {
                v.TemplateId = entity.Id;
                _variables.Add(v);
            }
        }
        return Task.FromResult(entity);
    }

    public void Update(NotificationTemplate entity)
    {
        if (entity.Versions is not null)
        {
            var existingIds = _versions.Select(v => v.Id).ToHashSet();
            foreach (var v in entity.Versions)
            {
                if (!existingIds.Contains(v.Id))
                {
                    v.TemplateId = entity.Id;
                    _versions.Add(v);
                }
            }
        }
        if (entity.Variables is not null)
        {
            var existingIds = _variables.Select(v => v.Id).ToHashSet();
            foreach (var v in entity.Variables)
            {
                if (!existingIds.Contains(v.Id))
                {
                    v.TemplateId = entity.Id;
                    _variables.Add(v);
                }
            }
        }
    }

    public void Remove(NotificationTemplate entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationTemplate, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryPreferenceRepository : IPreferenceRepository
{
    private readonly List<NotificationPreference> _entities;
    public IReadOnlyList<NotificationPreference> Items => _entities.AsReadOnly();
    public InMemoryPreferenceRepository(List<NotificationPreference> entities) => _entities = entities;

    public Task<NotificationPreference?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationPreference>>(
            _entities.Where(e => e.UserId == userId && !e.IsDeleted).ToList());

    public Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e =>
            e.UserId == userId && e.ChannelType == channelType && !e.IsDeleted));

    public Task<bool> IsChannelEnabledAsync(Guid userId, NotificationChannelType channelType, CancellationToken ct = default)
    {
        var pref = _entities.FirstOrDefault(e =>
            e.UserId == userId && e.ChannelType == channelType && !e.IsDeleted);
        return Task.FromResult(pref?.IsEnabled ?? true);
    }

    public Task<IReadOnlyList<NotificationPreference>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationPreference>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationPreference>> FindAsync(Expression<Func<NotificationPreference, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationPreference>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationPreference> AddAsync(NotificationPreference entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(NotificationPreference entity) { }

    public void Remove(NotificationPreference entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationPreference, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationPreference, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryDeliveryRepository : IDeliveryRepository
{
    private readonly List<NotificationDelivery> _entities;
    private readonly List<NotificationRetry> _retries;

    public IReadOnlyList<NotificationDelivery> Items => _entities.AsReadOnly();

    public InMemoryDeliveryRepository(List<NotificationDelivery> entities, List<NotificationRetry> retries)
    {
        _entities = entities;
        _retries = retries;
    }

    public Task<NotificationDelivery?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<NotificationDelivery?> GetByProviderMessageIdAsync(string providerMessageId, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e =>
            e.ProviderMessageId == providerMessageId && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationDelivery>> GetByNotificationIdAsync(Guid notificationId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationDelivery>>(
            _entities.Where(e => e.NotificationId == notificationId && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationDelivery>> GetByStatusAsync(NotificationStatus status, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationDelivery>>(
            _entities.Where(e => e.Status == status && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationDelivery>> GetFailedDeliveriesAsync(int maxRetries, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationDelivery>>(
            _entities.Where(e => e.Status == NotificationStatus.Failed && e.AttemptCount < maxRetries && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationDelivery>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationDelivery>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationDelivery>> FindAsync(Expression<Func<NotificationDelivery, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationDelivery>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationDelivery> AddAsync(NotificationDelivery entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(NotificationDelivery entity) { }

    public void Remove(NotificationDelivery entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationDelivery, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationDelivery, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryQueueRepository : IQueueRepository
{
    private readonly List<NotificationQueue> _entities;
    public IReadOnlyList<NotificationQueue> Items => _entities.AsReadOnly();
    public InMemoryQueueRepository(List<NotificationQueue> entities) => _entities = entities;

    public Task<NotificationQueue?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationQueue>> GetPendingItemsAsync(int take, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => e.Status == NotificationStatus.Queued && !e.IsDeleted)
                .OrderBy(e => e.Priority).ThenBy(e => e.QueuedAt).Take(take).ToList());

    public Task<IReadOnlyList<NotificationQueue>> GetByStatusAsync(NotificationStatus status, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => e.Status == status && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationQueue>> GetByPriorityAsync(NotificationPriority priority, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => e.Priority == priority && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationQueue>> GetStaleLocksAsync(DateTime threshold, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => e.LockExpiresAt < threshold && !e.IsDeleted).ToList());

    public Task<NotificationQueue?> GetByNotificationIdAsync(Guid notificationId, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.NotificationId == notificationId && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationQueue>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationQueue>> FindAsync(Expression<Func<NotificationQueue, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationQueue>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationQueue> AddAsync(NotificationQueue entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(NotificationQueue entity) { }

    public void Remove(NotificationQueue entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationQueue, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationQueue, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryAuditRepository : IAuditRepository
{
    private readonly List<NotificationAudit> _entities;
    public IReadOnlyList<NotificationAudit> Items => _entities.AsReadOnly();
    public InMemoryAuditRepository(List<NotificationAudit> entities) => _entities = entities;

    public Task<NotificationAudit?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationAudit>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationAudit>>(
            _entities.Where(e => e.EntityType == entityType && e.EntityId == entityId && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationAudit>> GetByActionAsync(string action, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationAudit>>(
            _entities.Where(e => e.Action == action && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationAudit>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationAudit>>(
            _entities.Where(e => e.ChangedAt >= from && e.ChangedAt <= to && !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationAudit>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationAudit>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationAudit>> FindAsync(Expression<Func<NotificationAudit, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationAudit>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationAudit> AddAsync(NotificationAudit entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(NotificationAudit entity) { }

    public void Remove(NotificationAudit entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationAudit, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationAudit, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryCampaignRepository : IRepository<NotificationCampaign>
{
    private readonly List<NotificationCampaign> _entities;
    public IReadOnlyList<NotificationCampaign> Items => _entities.AsReadOnly();
    public InMemoryCampaignRepository(List<NotificationCampaign> entities) => _entities = entities;

    public Task<NotificationCampaign?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<IReadOnlyList<NotificationCampaign>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationCampaign>>(
            _entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<NotificationCampaign>> FindAsync(Expression<Func<NotificationCampaign, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<NotificationCampaign>>(
            _entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<NotificationCampaign> AddAsync(NotificationCampaign entity, CancellationToken ct = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(NotificationCampaign entity) { }

    public void Remove(NotificationCampaign entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<NotificationCampaign, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<NotificationCampaign, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

#endregion

#region Mock Services

public class MockNotificationDispatcher : INotificationDispatcher
{
    public List<(Guid NotificationId, Guid? RecipientId)> DispatchedItems { get; } = new();

    public Task<Result<bool>> DispatchAsync(Guid notificationId, CancellationToken ct = default)
    {
        DispatchedItems.Add((notificationId, null));
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> DispatchToRecipientAsync(Guid notificationId, Guid recipientId, CancellationToken ct = default)
    {
        DispatchedItems.Add((notificationId, recipientId));
        return Task.FromResult(Result<bool>.Success(true));
    }

    public void Reset() => DispatchedItems.Clear();
}

public class MockQueueService : IQueueService
{
    public List<Guid> EnqueuedItems { get; } = new();
    public List<Guid> DequeuedItems { get; } = new();

    public Task<Result<bool>> EnqueueAsync(Guid notificationId, CancellationToken ct = default)
    {
        EnqueuedItems.Add(notificationId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> DequeueAsync(Guid notificationId, CancellationToken ct = default)
    {
        DequeuedItems.Add(notificationId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> MarkProcessingAsync(Guid queueId, string lockToken, CancellationToken ct = default)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> MarkCompletedAsync(Guid queueId, CancellationToken ct = default)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> MarkFailedAsync(Guid queueId, CancellationToken ct = default)
        => Task.FromResult(Result<bool>.Success(true));

    public void Reset()
    {
        EnqueuedItems.Clear();
        DequeuedItems.Clear();
    }
}

public class MockRecipientResolver : IRecipientResolver
{
    public List<ResolvedRecipient> ResolvedRecipients { get; set; } = new()
    {
        new ResolvedRecipient(Guid.Parse("30000000-0000-0000-0000-000000000001"), "Email", "test@example.com", "Test User")
    };

    public Task<Result<List<ResolvedRecipient>>> ResolveAsync(Guid? userId, string destinationAddress, string? recipientName, CancellationToken ct = default)
        => Task.FromResult(Result<List<ResolvedRecipient>>.Success(ResolvedRecipients));

    public Task<Result<List<ResolvedRecipient>>> ResolveByCriteriaAsync(string criteria, CancellationToken ct = default)
        => Task.FromResult(Result<List<ResolvedRecipient>>.Success(ResolvedRecipients));

    public void Reset() => ResolvedRecipients.Clear();
}

public class MockTemplateRenderer : ITemplateRenderer
{
    public string RenderedSubject { get; set; } = "Test Subject";
    public string RenderedBody { get; set; } = "<p>Test Body</p>";
    public List<(string Template, string Content)> RenderedItems { get; } = new();

    public Task<Result<(string Subject, string Body)>> RenderAsync(
        string subjectTemplate, string bodyTemplate,
        IReadOnlyDictionary<string, string> variables, CancellationToken ct = default)
    {
        RenderedItems.Add((subjectTemplate, bodyTemplate));
        return Task.FromResult(Result<(string Subject, string Body)>.Success((RenderedSubject, RenderedBody)));
    }

    public IReadOnlyList<string> ExtractVariables(string template)
        => Array.Empty<string>();

    public void Reset()
    {
        RenderedItems.Clear();
        RenderedSubject = "Test Subject";
        RenderedBody = "<p>Test Body</p>";
    }
}

public class MockBusinessRuleValidator : IBusinessRuleValidator
{
    public bool ShouldFail { get; set; }
    public string FailureMessage { get; set; } = "Business rule validation failed";

    public Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken ct = default)
    {
        if (ShouldFail)
            return Task.FromResult(Result<bool>.Failure(FailureMessage));
        return Task.FromResult(Result<bool>.Success(true));
    }

    public void Reset()
    {
        ShouldFail = false;
        FailureMessage = "Business rule validation failed";
    }
}

#endregion
