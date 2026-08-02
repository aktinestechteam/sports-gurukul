using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class KnowledgeBaseRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesSources()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), Name = "KB", CreatedAt = DateTime.UtcNow };
        var source = new KnowledgeSource { Id = Guid.NewGuid(), KnowledgeBaseId = kb.Id, Name = "S", CreatedAt = DateTime.UtcNow };
        context.KnowledgeBases.Add(kb);
        context.KnowledgeSources.Add(source);
        await context.SaveChangesAsync();

        var repo = new KnowledgeBaseRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(kb.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Sources.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByVisibilityAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeBases.AddRange(
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "a", Visibility = KnowledgeBaseVisibility.Public, CreatedAt = DateTime.UtcNow },
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "b", Visibility = KnowledgeBaseVisibility.Private, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeBaseRepository(context);
        var result = await repo.GetByVisibilityAsync("Public", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeBases.AddRange(
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "a", Status = KnowledgeBaseStatus.Published, CreatedAt = DateTime.UtcNow },
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "b", Status = KnowledgeBaseStatus.Draft, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeBaseRepository(context);
        var result = await repo.GetByStatusAsync("Published", CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Status.Should().Be(KnowledgeBaseStatus.Published);
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsOnlyPublic()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeBases.AddRange(
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "a", Visibility = KnowledgeBaseVisibility.Public, CreatedAt = DateTime.UtcNow },
            new KnowledgeBase { Id = Guid.NewGuid(), Name = "b", Visibility = KnowledgeBaseVisibility.Private, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeBaseRepository(context);
        var result = await repo.GetPublicAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class KnowledgeSourceRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), Name = "KB", CreatedAt = DateTime.UtcNow };
        var source = new KnowledgeSource { Id = Guid.NewGuid(), KnowledgeBaseId = kb.Id, Name = "S", CreatedAt = DateTime.UtcNow };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid(), KnowledgeSourceId = source.Id, Title = "D", CreatedAt = DateTime.UtcNow };
        context.KnowledgeBases.Add(kb);
        context.KnowledgeSources.Add(source);
        context.KnowledgeDocuments.Add(doc);
        await context.SaveChangesAsync();

        var repo = new KnowledgeSourceRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(source.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.KnowledgeBase.Should().NotBeNull();
        loaded.Documents.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByKnowledgeBaseIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var kbA = Guid.NewGuid();
        context.KnowledgeSources.AddRange(
            new KnowledgeSource { Id = Guid.NewGuid(), KnowledgeBaseId = kbA, Name = "a", CreatedAt = DateTime.UtcNow },
            new KnowledgeSource { Id = Guid.NewGuid(), KnowledgeBaseId = kbA, Name = "b", CreatedAt = DateTime.UtcNow },
            new KnowledgeSource { Id = Guid.NewGuid(), KnowledgeBaseId = Guid.NewGuid(), Name = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeSourceRepository(context);
        var result = await repo.GetByKnowledgeBaseIdAsync(kbA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBySourceTypeAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeSources.AddRange(
            new KnowledgeSource { Id = Guid.NewGuid(), Name = "a", SourceType = KnowledgeSourceType.WebPage, CreatedAt = DateTime.UtcNow },
            new KnowledgeSource { Id = Guid.NewGuid(), Name = "b", SourceType = KnowledgeSourceType.Document, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeSourceRepository(context);
        var result = await repo.GetBySourceTypeAsync("WebPage", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeSources.AddRange(
            new KnowledgeSource { Id = Guid.NewGuid(), Name = "a", Status = SourceStatus.Indexed, CreatedAt = DateTime.UtcNow },
            new KnowledgeSource { Id = Guid.NewGuid(), Name = "b", Status = SourceStatus.Pending, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeSourceRepository(context);
        var result = await repo.GetByStatusAsync("Indexed", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class KnowledgeDocumentRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var source = new KnowledgeSource { Id = Guid.NewGuid(), Name = "S", CreatedAt = DateTime.UtcNow };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid(), KnowledgeSourceId = source.Id, Title = "D", CreatedAt = DateTime.UtcNow };
        var embedding = new Embedding { Id = Guid.NewGuid(), DocumentId = doc.Id, ModelName = "m", CreatedAt = DateTime.UtcNow };
        context.KnowledgeSources.Add(source);
        context.KnowledgeDocuments.Add(doc);
        context.Embeddings.Add(embedding);
        await context.SaveChangesAsync();

        var repo = new KnowledgeDocumentRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(doc.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.KnowledgeSource.Should().NotBeNull();
        loaded.Embeddings.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByKnowledgeSourceIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var sourceA = Guid.NewGuid();
        context.KnowledgeDocuments.AddRange(
            new KnowledgeDocument { Id = Guid.NewGuid(), KnowledgeSourceId = sourceA, Title = "a", CreatedAt = DateTime.UtcNow },
            new KnowledgeDocument { Id = Guid.NewGuid(), KnowledgeSourceId = sourceA, Title = "b", CreatedAt = DateTime.UtcNow },
            new KnowledgeDocument { Id = Guid.NewGuid(), KnowledgeSourceId = Guid.NewGuid(), Title = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeDocumentRepository(context);
        var result = await repo.GetByKnowledgeSourceIdAsync(sourceA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeDocuments.AddRange(
            new KnowledgeDocument { Id = Guid.NewGuid(), Title = "a", EmbeddingStatus = EmbeddingStatus.Completed, CreatedAt = DateTime.UtcNow },
            new KnowledgeDocument { Id = Guid.NewGuid(), Title = "b", EmbeddingStatus = EmbeddingStatus.Pending, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeDocumentRepository(context);
        var result = await repo.GetByStatusAsync("Completed", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByDocumentTypeAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.KnowledgeDocuments.AddRange(
            new KnowledgeDocument { Id = Guid.NewGuid(), Title = "a", Type = DocumentType.Pdf, CreatedAt = DateTime.UtcNow },
            new KnowledgeDocument { Id = Guid.NewGuid(), Title = "b", Type = DocumentType.Markdown, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new KnowledgeDocumentRepository(context);
        var result = await repo.GetByDocumentTypeAsync("Pdf", CancellationToken.None);

        result.Should().ContainSingle();
    }
}
