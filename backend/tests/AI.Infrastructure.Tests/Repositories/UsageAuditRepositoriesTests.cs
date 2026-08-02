using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class AITokenUsageRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "C", CreatedAt = DateTime.UtcNow };
        var message = new ConversationMessage { Id = Guid.NewGuid(), ConversationId = conversation.Id, Content = "m", CreatedAt = DateTime.UtcNow };
        var usage = new AITokenUsage { Id = Guid.NewGuid(), ConversationId = conversation.Id, MessageId = message.Id, ModelName = "gpt-4", CreatedAt = DateTime.UtcNow };
        context.Conversations.Add(conversation);
        context.ConversationMessages.Add(message);
        context.AITokenUsages.Add(usage);
        await context.SaveChangesAsync();

        var repo = new AITokenUsageRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(usage.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Conversation.Should().NotBeNull();
        loaded.Message.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByConversationIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var convA = Guid.NewGuid();
        context.AITokenUsages.AddRange(
            new AITokenUsage { Id = Guid.NewGuid(), ConversationId = convA, ModelName = "m", CreatedAt = DateTime.UtcNow },
            new AITokenUsage { Id = Guid.NewGuid(), ConversationId = convA, ModelName = "m", CreatedAt = DateTime.UtcNow },
            new AITokenUsage { Id = Guid.NewGuid(), ConversationId = Guid.NewGuid(), ModelName = "m", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AITokenUsageRepository(context);
        var result = await repo.GetByConversationIdAsync(convA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByUserIdAsync_MatchesStoredString()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var userId = Guid.NewGuid();
        context.AITokenUsages.AddRange(
            new AITokenUsage { Id = Guid.NewGuid(), UserId = userId.ToString(), ModelName = "m", CreatedAt = DateTime.UtcNow },
            new AITokenUsage { Id = Guid.NewGuid(), UserId = userId.ToString(), ModelName = "m", CreatedAt = DateTime.UtcNow },
            new AITokenUsage { Id = Guid.NewGuid(), UserId = Guid.NewGuid().ToString(), ModelName = "m", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AITokenUsageRepository(context);
        var result = await repo.GetByUserIdAsync(userId, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByDateRangeAsync_FiltersByCreatedAt()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);
        var inRange = new AITokenUsage { Id = Guid.NewGuid(), ModelName = "m" };
        var before = new AITokenUsage { Id = Guid.NewGuid(), ModelName = "m" };
        var after = new AITokenUsage { Id = Guid.NewGuid(), ModelName = "m" };
        context.AITokenUsages.AddRange(inRange, before, after);
        await context.SaveChangesAsync();
        inRange.CreatedAt = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        before.CreatedAt = new DateTime(2025, 12, 31, 23, 0, 0, DateTimeKind.Utc);
        after.CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        await context.SaveChangesAsync();

        var repo = new AITokenUsageRepository(context);
        var result = await repo.GetByDateRangeAsync(start, end, CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class AIAuditLogRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_ReturnsLog()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var log = new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", EventType = AuditEventType.Create, Message = "created", CreatedAt = DateTime.UtcNow };
        context.AIAuditLogs.Add(log);
        await context.SaveChangesAsync();

        var repo = new AIAuditLogRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(log.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Message.Should().Be("created");
    }

    [Fact]
    public async Task GetByEntityIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var entityA = Guid.NewGuid();
        context.AIAuditLogs.AddRange(
            new AIAuditLog { Id = Guid.NewGuid(), EntityId = entityA, EntityType = "Agent", EventType = AuditEventType.Create, CreatedAt = DateTime.UtcNow },
            new AIAuditLog { Id = Guid.NewGuid(), EntityId = entityA, EntityType = "Agent", EventType = AuditEventType.Update, CreatedAt = DateTime.UtcNow },
            new AIAuditLog { Id = Guid.NewGuid(), EntityId = Guid.NewGuid(), EntityType = "Agent", EventType = AuditEventType.Create, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIAuditLogRepository(context);
        var result = await repo.GetByEntityIdAsync(entityA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByEventTypeAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIAuditLogs.AddRange(
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", EventType = AuditEventType.Delete, CreatedAt = DateTime.UtcNow },
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", EventType = AuditEventType.Create, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIAuditLogRepository(context);
        var result = await repo.GetByEventTypeAsync("Delete", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetRecentBySeverityAsync_ParsesFiltersOrdersAndLimits()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIAuditLogs.AddRange(
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", Severity = AuditSeverity.Error },
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", Severity = AuditSeverity.Error },
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", Severity = AuditSeverity.Error },
            new AIAuditLog { Id = Guid.NewGuid(), EntityType = "Agent", Severity = AuditSeverity.Info });
        await context.SaveChangesAsync();
        var errorLogs = context.AIAuditLogs.Where(l => l.Severity == AuditSeverity.Error).ToList();
        errorLogs[0].CreatedAt = DateTime.UtcNow.AddMinutes(-5);
        errorLogs[1].CreatedAt = DateTime.UtcNow;
        errorLogs[2].CreatedAt = DateTime.UtcNow.AddMinutes(-2);
        await context.SaveChangesAsync();

        var repo = new AIAuditLogRepository(context);
        var result = await repo.GetRecentBySeverityAsync("Error", 2, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].CreatedAt.Should().Be(result.Max(l => l.CreatedAt));
    }
}
