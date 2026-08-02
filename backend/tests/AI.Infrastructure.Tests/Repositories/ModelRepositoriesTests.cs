using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class AIProviderRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesModels()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var provider = new AIProvider { Id = Guid.NewGuid(), Name = "OpenAI", CreatedAt = DateTime.UtcNow };
        var model = new AIModel { Id = Guid.NewGuid(), DisplayName = "gpt-4", ProviderId = provider.Id, CreatedAt = DateTime.UtcNow };
        context.AIProviders.Add(provider);
        context.AIModels.Add(model);
        await context.SaveChangesAsync();

        var repo = new AIProviderRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(provider.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Models.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIProviders.AddRange(
            new AIProvider { Id = Guid.NewGuid(), Name = "a", IsActive = true, CreatedAt = DateTime.UtcNow },
            new AIProvider { Id = Guid.NewGuid(), Name = "b", IsActive = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIProviderRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByTypeAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIProviders.AddRange(
            new AIProvider { Id = Guid.NewGuid(), Name = "a", Type = AIProviderType.OpenAI, CreatedAt = DateTime.UtcNow },
            new AIProvider { Id = Guid.NewGuid(), Name = "b", Type = AIProviderType.Anthropic, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIProviderRepository(context);
        var result = await repo.GetByTypeAsync("OpenAI", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class AIModelRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var provider = new AIProvider { Id = Guid.NewGuid(), Name = "OpenAI", CreatedAt = DateTime.UtcNow };
        var model = new AIModel { Id = Guid.NewGuid(), DisplayName = "gpt-4", ProviderId = provider.Id, CreatedAt = DateTime.UtcNow };
        var config = new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = model.Id, DisplayName = "c", CreatedAt = DateTime.UtcNow };
        context.AIProviders.Add(provider);
        context.AIModels.Add(model);
        context.AIModelConfigurations.Add(config);
        await context.SaveChangesAsync();

        var repo = new AIModelRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(model.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Provider.Should().NotBeNull();
        loaded.ModelConfigurations.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByProviderAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var providerA = Guid.NewGuid();
        context.AIModels.AddRange(
            new AIModel { Id = Guid.NewGuid(), DisplayName = "a", ProviderId = providerA, CreatedAt = DateTime.UtcNow },
            new AIModel { Id = Guid.NewGuid(), DisplayName = "b", ProviderId = providerA, CreatedAt = DateTime.UtcNow },
            new AIModel { Id = Guid.NewGuid(), DisplayName = "c", ProviderId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIModelRepository(context);
        var result = await repo.GetByProviderAsync(providerA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIModels.AddRange(
            new AIModel { Id = Guid.NewGuid(), DisplayName = "a", Status = AIModelStatus.Active, CreatedAt = DateTime.UtcNow },
            new AIModel { Id = Guid.NewGuid(), DisplayName = "b", Status = AIModelStatus.Deprecated, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIModelRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByCapabilityAsync_FiltersByFlag()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIModels.AddRange(
            new AIModel { Id = Guid.NewGuid(), DisplayName = "a", Capabilities = AIModelCapability.TextGeneration, CreatedAt = DateTime.UtcNow },
            new AIModel { Id = Guid.NewGuid(), DisplayName = "b", Capabilities = AIModelCapability.TextGeneration | AIModelCapability.ImageGeneration, CreatedAt = DateTime.UtcNow },
            new AIModel { Id = Guid.NewGuid(), DisplayName = "c", Capabilities = AIModelCapability.Embedding, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIModelRepository(context);
        var result = await repo.GetByCapabilityAsync("TextGeneration", CancellationToken.None);

        result.Should().HaveCount(2);
    }
}

public class AIModelConfigurationRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesModel()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var model = new AIModel { Id = Guid.NewGuid(), DisplayName = "gpt-4", CreatedAt = DateTime.UtcNow };
        var config = new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = model.Id, DisplayName = "c", CreatedAt = DateTime.UtcNow };
        context.AIModels.Add(model);
        context.AIModelConfigurations.Add(config);
        await context.SaveChangesAsync();

        var repo = new AIModelConfigurationRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(config.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Model.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByModelIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var modelA = Guid.NewGuid();
        context.AIModelConfigurations.AddRange(
            new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = modelA, DisplayName = "a", CreatedAt = DateTime.UtcNow },
            new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = modelA, DisplayName = "b", CreatedAt = DateTime.UtcNow },
            new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = Guid.NewGuid(), DisplayName = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIModelConfigurationRepository(context);
        var result = await repo.GetByModelIdAsync(modelA, CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetDefaultForModelAsync_ReturnsOnlyDefault()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var modelA = Guid.NewGuid();
        context.AIModelConfigurations.AddRange(
            new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = modelA, DisplayName = "a", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new AIModelConfiguration { Id = Guid.NewGuid(), ModelId = modelA, DisplayName = "b", IsDefault = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIModelConfigurationRepository(context);
        var result = await repo.GetDefaultForModelAsync(modelA, CancellationToken.None);

        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("a");
    }

    [Fact]
    public async Task GetDefaultForModelAsync_NoDefault_ReturnsNull()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var repo = new AIModelConfigurationRepository(context);
        var result = await repo.GetDefaultForModelAsync(Guid.NewGuid(), CancellationToken.None);
        result.Should().BeNull();
    }
}
