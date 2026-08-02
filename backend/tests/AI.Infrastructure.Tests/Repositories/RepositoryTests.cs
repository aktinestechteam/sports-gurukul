using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories;

namespace AI.Infrastructure.Tests.Repositories;

public class RepositoryTests
{
    private static Repository<Conversation> CreateRepo(ApplicationDbContext context) => new(context);

    [Fact]
    public async Task AddAsync_StoresEntity()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var repo = CreateRepo(context);
        var entity = new Conversation { Id = Guid.NewGuid(), Title = "one" };

        var added = await repo.AddAsync(entity, CancellationToken.None);
        await context.SaveChangesAsync();

        added.Id.Should().Be(entity.Id);
        var loaded = await context.Conversations.FindAsync(entity.Id);
        loaded.Should().NotBeNull();
        loaded!.Title.Should().Be("one");
        loaded.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var entity = new Conversation { Id = Guid.NewGuid(), Title = "one" };
        context.Conversations.Add(entity);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var loaded = await repo.GetByIdAsync(entity.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Title.Should().Be("one");
    }

    [Fact]
    public async Task GetByIdAsync_DeletedEntity_ReturnsNull()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var entity = new Conversation { Id = Guid.NewGuid(), Title = "one", IsDeleted = true };
        context.Conversations.Add(entity);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var loaded = await repo.GetByIdAsync(entity.Id, CancellationToken.None);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ExcludesDeleted()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Title = "a" },
            new Conversation { Id = Guid.NewGuid(), Title = "b", IsDeleted = true });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var all = await repo.GetAllAsync(CancellationToken.None);

        all.Should().ContainSingle();
        all[0].Title.Should().Be("a");
    }

    [Fact]
    public async Task FindAsync_AppliesPredicateAndExcludesDeleted()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Title = "match" },
            new Conversation { Id = Guid.NewGuid(), Title = "match", IsDeleted = true },
            new Conversation { Id = Guid.NewGuid(), Title = "other" });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var found = await repo.FindAsync(e => e.Title == "match", CancellationToken.None);

        found.Should().ContainSingle();
    }

    [Fact]
    public async Task Update_SavesChanges()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var entity = new Conversation { Id = Guid.NewGuid(), Title = "old" };
        context.Conversations.Add(entity);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        entity.Title = "new";
        repo.Update(entity);
        await context.SaveChangesAsync();

        var loaded = await context.Conversations.FindAsync(entity.Id);
        loaded!.Title.Should().Be("new");
    }

    [Fact]
    public async Task Remove_SoftDeletesEntity()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var entity = new Conversation { Id = Guid.NewGuid(), Title = "x" };
        context.Conversations.Add(entity);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        repo.Remove(entity);
        await context.SaveChangesAsync();

        var loaded = await context.Conversations.FindAsync(entity.Id);
        loaded.Should().NotBeNull();
        loaded!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CountAsync_WithPredicate_CountsNonDeleted()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Title = "a" },
            new Conversation { Id = Guid.NewGuid(), Title = "b" },
            new Conversation { Id = Guid.NewGuid(), Title = "b", IsDeleted = true });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var count = await repo.CountAsync(e => e.Title == "b", CancellationToken.None);

        count.Should().Be(1);
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_CountsAllNonDeleted()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.AddRange(
            new Conversation { Id = Guid.NewGuid(), Title = "a" },
            new Conversation { Id = Guid.NewGuid(), Title = "b" });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var count = await repo.CountAsync(null, CancellationToken.None);

        count.Should().Be(2);
    }

    [Fact]
    public async Task AnyAsync_ReturnsTrueWhenMatchExists()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.Add(new Conversation { Id = Guid.NewGuid(), Title = "a" });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var exists = await repo.AnyAsync(e => e.Title == "a", CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_IgnoresDeleted()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Conversations.Add(new Conversation { Id = Guid.NewGuid(), Title = "a", IsDeleted = true });
        await context.SaveChangesAsync();

        var repo = CreateRepo(context);
        var exists = await repo.AnyAsync(e => e.Title == "a", CancellationToken.None);

        exists.Should().BeFalse();
    }
}
