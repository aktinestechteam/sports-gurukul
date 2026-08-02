using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class ConversationRepositoryTests
{
    private static ConversationRepository CreateRepo(ApplicationDbContext context) => new(context);

    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", CreatedAt = DateTime.UtcNow };
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "T", AssistantId = assistant.Id, UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var message = new ConversationMessage { Id = Guid.NewGuid(), ConversationId = conversation.Id, Role = MessageRole.User, Content = "hi", CreatedAt = DateTime.UtcNow };
        var memory = new ConversationMemory { Id = Guid.NewGuid(), ConversationId = conversation.Id, Type = MemoryType.ShortTerm, Content = "m", CreatedAt = DateTime.UtcNow };
        context.AIAssistants.Add(assistant);
        context.Conversations.Add(conversation);
        context.ConversationMessages.Add(message);
        context.ConversationMemories.Add(memory);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var loaded = await repo.GetByIdWithDetailsAsync(conversation.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Assistant.Should().NotBeNull();
        loaded.Messages.Should().ContainSingle();
        loaded.Memories.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_MissingId_ReturnsNull()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var repo = CreateRepo(context);
        var loaded = await repo.GetByIdWithDetailsAsync(Guid.NewGuid(), CancellationToken.None);
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), UserId = userA, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), UserId = userA, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), UserId = userB, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var result = await repo.GetByUserIdAsync(userA, CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByAssistantIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var assistantA = Guid.NewGuid();
        var assistantB = Guid.NewGuid();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), AssistantId = assistantA, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), AssistantId = assistantB, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var result = await repo.GetByAssistantIdAsync(assistantA, CancellationToken.None);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Active, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Archived, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Completed, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);
        result.Should().ContainSingle();
        result[0].Status.Should().Be(ConversationStatus.Active);
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesStatusAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Archived, CreatedAt = DateTime.UtcNow },
            new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Active, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var result = await repo.GetByStatusAsync("Archived", CancellationToken.None);
        result.Should().ContainSingle();
        result[0].Status.Should().Be(ConversationStatus.Archived);
    }
}

public class ConversationMessageRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesConversation()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "T", CreatedAt = DateTime.UtcNow };
        var message = new ConversationMessage { Id = Guid.NewGuid(), ConversationId = conversation.Id, Role = MessageRole.User, Content = "hi", CreatedAt = DateTime.UtcNow };
        context.Conversations.Add(conversation);
        context.ConversationMessages.Add(message);
        await context.SaveChangesAsync();

        var repo = new ConversationMessageRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(message.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Conversation.Should().NotBeNull();
        loaded.Conversation!.Title.Should().Be("T");
    }

    [Fact]
    public async Task GetByConversationIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var convA = Guid.NewGuid();
        var convB = Guid.NewGuid();
        context.ConversationMessages.AddRange(
            new ConversationMessage { Id = Guid.NewGuid(), ConversationId = convA, Content = "a", CreatedAt = DateTime.UtcNow },
            new ConversationMessage { Id = Guid.NewGuid(), ConversationId = convA, Content = "b", CreatedAt = DateTime.UtcNow },
            new ConversationMessage { Id = Guid.NewGuid(), ConversationId = convB, Content = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ConversationMessageRepository(context);
        var result = await repo.GetByConversationIdAsync(convA, CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentByConversationIdAsync_OrdersDescendingAndLimits()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var conv = Guid.NewGuid();
        var older = new ConversationMessage { Id = Guid.NewGuid(), ConversationId = conv, Content = "old", CreatedAt = DateTime.UtcNow.AddMinutes(-10) };
        var newer = new ConversationMessage { Id = Guid.NewGuid(), ConversationId = conv, Content = "new", CreatedAt = DateTime.UtcNow };
        context.ConversationMessages.AddRange(older, newer);
        await context.SaveChangesAsync();
        older.CreatedAt = DateTime.UtcNow.AddMinutes(-10);
        newer.CreatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var repo = new ConversationMessageRepository(context);
        var result = await repo.GetRecentByConversationIdAsync(conv, 1, CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Content.Should().Be("new");
    }
}

public class ConversationMemoryRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesConversation()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "T", CreatedAt = DateTime.UtcNow };
        var memory = new ConversationMemory { Id = Guid.NewGuid(), ConversationId = conversation.Id, Content = "m", CreatedAt = DateTime.UtcNow };
        context.Conversations.Add(conversation);
        context.ConversationMemories.Add(memory);
        await context.SaveChangesAsync();

        var repo = new ConversationMemoryRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(memory.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Conversation.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByConversationIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var convA = Guid.NewGuid();
        context.ConversationMemories.AddRange(
            new ConversationMemory { Id = Guid.NewGuid(), ConversationId = convA, Content = "a", CreatedAt = DateTime.UtcNow },
            new ConversationMemory { Id = Guid.NewGuid(), ConversationId = convA, Content = "b", CreatedAt = DateTime.UtcNow },
            new ConversationMemory { Id = Guid.NewGuid(), ConversationId = Guid.NewGuid(), Content = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ConversationMemoryRepository(context);
        var result = await repo.GetByConversationIdAsync(convA, CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByTypeAndImportanceAsync_FiltersByTypeAndImportance()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.ConversationMemories.AddRange(
            new ConversationMemory { Id = Guid.NewGuid(), Type = MemoryType.LongTerm, Importance = MemoryImportance.High, Content = "a", CreatedAt = DateTime.UtcNow },
            new ConversationMemory { Id = Guid.NewGuid(), Type = MemoryType.LongTerm, Importance = MemoryImportance.Low, Content = "b", CreatedAt = DateTime.UtcNow },
            new ConversationMemory { Id = Guid.NewGuid(), Type = MemoryType.ShortTerm, Importance = MemoryImportance.High, Content = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ConversationMemoryRepository(context);
        var result = await repo.GetByTypeAndImportanceAsync("LongTerm", (int)MemoryImportance.Normal, CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Content.Should().Be("a");
    }
}
