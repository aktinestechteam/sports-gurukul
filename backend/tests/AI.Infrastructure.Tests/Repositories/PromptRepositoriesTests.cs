using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class PromptTemplateRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesVersions()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var template = new PromptTemplate { Id = Guid.NewGuid(), Name = "P", Type = PromptType.Template, CreatedAt = DateTime.UtcNow };
        var version = new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template.Id, VersionNumber = 1, Content = "c", CreatedAt = DateTime.UtcNow };
        context.PromptTemplates.Add(template);
        context.PromptVersions.Add(version);
        await context.SaveChangesAsync();

        var repo = new PromptTemplateRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(template.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Versions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.PromptTemplates.AddRange(
            new PromptTemplate { Id = Guid.NewGuid(), Name = "a", Status = PromptStatus.Active, CreatedAt = DateTime.UtcNow },
            new PromptTemplate { Id = Guid.NewGuid(), Name = "b", Status = PromptStatus.Draft, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new PromptTemplateRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Name.Should().Be("a");
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.PromptTemplates.AddRange(
            new PromptTemplate { Id = Guid.NewGuid(), Name = "a", Category = "sports", CreatedAt = DateTime.UtcNow },
            new PromptTemplate { Id = Guid.NewGuid(), Name = "b", Category = "other", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new PromptTemplateRepository(context);
        var result = await repo.GetByCategoryAsync("sports", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByTypeAsync_ParsesTypeAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.PromptTemplates.AddRange(
            new PromptTemplate { Id = Guid.NewGuid(), Name = "a", Type = PromptType.System, CreatedAt = DateTime.UtcNow },
            new PromptTemplate { Id = Guid.NewGuid(), Name = "b", Type = PromptType.Template, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new PromptTemplateRepository(context);
        var result = await repo.GetByTypeAsync("System", CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Type.Should().Be(PromptType.System);
    }
}

public class PromptVersionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesPromptTemplate()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var template = new PromptTemplate { Id = Guid.NewGuid(), Name = "P", CreatedAt = DateTime.UtcNow };
        var version = new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template.Id, VersionNumber = 1, Content = "c", CreatedAt = DateTime.UtcNow };
        context.PromptTemplates.Add(template);
        context.PromptVersions.Add(version);
        await context.SaveChangesAsync();

        var repo = new PromptVersionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(version.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.PromptTemplate.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByTemplateIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var template = Guid.NewGuid();
        context.PromptVersions.AddRange(
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template, VersionNumber = 1, CreatedAt = DateTime.UtcNow },
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template, VersionNumber = 2, CreatedAt = DateTime.UtcNow },
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = Guid.NewGuid(), VersionNumber = 1, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new PromptVersionRepository(context);
        var result = await repo.GetByTemplateIdAsync(template, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLatestVersionAsync_ReturnsHighestVersion()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var template = Guid.NewGuid();
        context.PromptVersions.AddRange(
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template, VersionNumber = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template, VersionNumber = 5, CreatedAt = DateTime.UtcNow },
            new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template, VersionNumber = 3, CreatedAt = DateTime.UtcNow.AddDays(-1) });
        await context.SaveChangesAsync();

        var repo = new PromptVersionRepository(context);
        var latest = await repo.GetLatestVersionAsync(template, CancellationToken.None);

        latest.Should().NotBeNull();
        latest!.VersionNumber.Should().Be(5);
    }

    [Fact]
    public async Task GetLatestVersionAsync_NoVersions_ReturnsNull()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var repo = new PromptVersionRepository(context);
        var latest = await repo.GetLatestVersionAsync(Guid.NewGuid(), CancellationToken.None);
        latest.Should().BeNull();
    }
}
