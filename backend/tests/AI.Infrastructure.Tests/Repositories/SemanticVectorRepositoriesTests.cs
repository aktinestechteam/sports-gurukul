using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class VectorIndexRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_ReturnsIndex()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var index = new VectorIndex { Id = Guid.NewGuid(), Name = "I", CreatedAt = DateTime.UtcNow };
        context.VectorIndices.Add(index);
        await context.SaveChangesAsync();

        var repo = new VectorIndexRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(index.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("I");
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.VectorIndices.AddRange(
            new VectorIndex { Id = Guid.NewGuid(), Name = "a", Status = VectorIndexStatus.Active, CreatedAt = DateTime.UtcNow },
            new VectorIndex { Id = Guid.NewGuid(), Name = "b", Status = VectorIndexStatus.Building, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new VectorIndexRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.VectorIndices.AddRange(
            new VectorIndex { Id = Guid.NewGuid(), Name = "a", Status = VectorIndexStatus.Failed, CreatedAt = DateTime.UtcNow },
            new VectorIndex { Id = Guid.NewGuid(), Name = "b", Status = VectorIndexStatus.Active, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new VectorIndexRepository(context);
        var result = await repo.GetByStatusAsync("Failed", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class EmbeddingRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var doc = new KnowledgeDocument { Id = Guid.NewGuid(), Title = "D", CreatedAt = DateTime.UtcNow };
        var chunk = new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = doc.Id, ChunkIndex = 0, Content = "c", CreatedAt = DateTime.UtcNow };
        var embedding = new Embedding { Id = Guid.NewGuid(), DocumentId = doc.Id, ChunkId = chunk.Id, ModelName = "m", CreatedAt = DateTime.UtcNow };
        context.KnowledgeDocuments.Add(doc);
        context.EmbeddingChunks.Add(chunk);
        context.Embeddings.Add(embedding);
        await context.SaveChangesAsync();

        var repo = new EmbeddingRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(embedding.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Document.Should().NotBeNull();
        loaded.Chunk.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByDocumentIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var docA = Guid.NewGuid();
        context.Embeddings.AddRange(
            new Embedding { Id = Guid.NewGuid(), DocumentId = docA, ModelName = "m", CreatedAt = DateTime.UtcNow },
            new Embedding { Id = Guid.NewGuid(), DocumentId = docA, ModelName = "m", CreatedAt = DateTime.UtcNow },
            new Embedding { Id = Guid.NewGuid(), DocumentId = Guid.NewGuid(), ModelName = "m", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new EmbeddingRepository(context);
        var result = await repo.GetByDocumentIdAsync(docA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByModelNameAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.Embeddings.AddRange(
            new Embedding { Id = Guid.NewGuid(), ModelName = "text-embedding-3", CreatedAt = DateTime.UtcNow },
            new Embedding { Id = Guid.NewGuid(), ModelName = "text-embedding-3", CreatedAt = DateTime.UtcNow },
            new Embedding { Id = Guid.NewGuid(), ModelName = "other", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new EmbeddingRepository(context);
        var result = await repo.GetByModelNameAsync("text-embedding-3", CancellationToken.None);

        result.Should().HaveCount(2);
    }
}

public class EmbeddingChunkRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var doc = new KnowledgeDocument { Id = Guid.NewGuid(), Title = "D", CreatedAt = DateTime.UtcNow };
        var chunk = new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = doc.Id, ChunkIndex = 0, Content = "c", CreatedAt = DateTime.UtcNow };
        context.KnowledgeDocuments.Add(doc);
        context.EmbeddingChunks.Add(chunk);
        await context.SaveChangesAsync();

        var repo = new EmbeddingChunkRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(chunk.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Document.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByDocumentIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var docA = Guid.NewGuid();
        context.EmbeddingChunks.AddRange(
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 0, CreatedAt = DateTime.UtcNow },
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 1, CreatedAt = DateTime.UtcNow },
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = Guid.NewGuid(), ChunkIndex = 0, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new EmbeddingChunkRepository(context);
        var result = await repo.GetByDocumentIdAsync(docA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByChunkIndexAsync_FiltersRangeAndOrders()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var docA = Guid.NewGuid();
        context.EmbeddingChunks.AddRange(
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 0, CreatedAt = DateTime.UtcNow },
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 1, CreatedAt = DateTime.UtcNow },
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 2, CreatedAt = DateTime.UtcNow },
            new EmbeddingChunk { Id = Guid.NewGuid(), DocumentId = docA, ChunkIndex = 3, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new EmbeddingChunkRepository(context);
        var result = await repo.GetByChunkIndexAsync(docA, 1, 2, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(c => c.ChunkIndex).Should().BeInAscendingOrder();
    }
}

public class SemanticSearchRequestRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesResults()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var request = new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "q", CreatedAt = DateTime.UtcNow };
        var result = new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = request.Id, DocumentTitle = "D", CreatedAt = DateTime.UtcNow };
        context.SemanticSearchRequests.Add(request);
        context.SemanticSearchResults.Add(result);
        await context.SaveChangesAsync();

        var repo = new SemanticSearchRequestRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(request.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Results.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.SemanticSearchRequests.AddRange(
            new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "a", Status = SemanticSearchStatus.Completed, CreatedAt = DateTime.UtcNow },
            new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "b", Status = SemanticSearchStatus.Pending, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new SemanticSearchRequestRepository(context);
        var result = await repo.GetByStatusAsync("Completed", CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetRecentAsync_OrdersDescendingAndLimits()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.SemanticSearchRequests.AddRange(
            new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "a" },
            new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "b" },
            new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "c" });
        await context.SaveChangesAsync();
        var all = context.SemanticSearchRequests.ToList();
        all[0].CreatedAt = DateTime.UtcNow.AddMinutes(-10);
        all[1].CreatedAt = DateTime.UtcNow;
        all[2].CreatedAt = DateTime.UtcNow.AddMinutes(-5);
        await context.SaveChangesAsync();

        var repo = new SemanticSearchRequestRepository(context);
        var result = await repo.GetRecentAsync(2, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Query.Should().Be("b");
    }
}

public class SemanticSearchResultRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var request = new SemanticSearchRequest { Id = Guid.NewGuid(), Query = "q", CreatedAt = DateTime.UtcNow };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid(), Title = "D", CreatedAt = DateTime.UtcNow };
        var result = new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = request.Id, DocumentId = doc.Id, DocumentTitle = "D", CreatedAt = DateTime.UtcNow };
        context.SemanticSearchRequests.Add(request);
        context.KnowledgeDocuments.Add(doc);
        context.SemanticSearchResults.Add(result);
        await context.SaveChangesAsync();

        var repo = new SemanticSearchResultRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(result.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.SearchRequest.Should().NotBeNull();
        loaded.Document.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBySearchRequestIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var requestA = Guid.NewGuid();
        context.SemanticSearchResults.AddRange(
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = requestA, DocumentTitle = "a", CreatedAt = DateTime.UtcNow },
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = requestA, DocumentTitle = "b", CreatedAt = DateTime.UtcNow },
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = Guid.NewGuid(), DocumentTitle = "c", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new SemanticSearchResultRepository(context);
        var result = await repo.GetBySearchRequestIdAsync(requestA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByMinScoreAsync_FiltersByScoreAndOrdersDescending()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var requestA = Guid.NewGuid();
        context.SemanticSearchResults.AddRange(
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = requestA, DocumentTitle = "a", Score = 0.95, CreatedAt = DateTime.UtcNow },
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = requestA, DocumentTitle = "b", Score = 0.5, CreatedAt = DateTime.UtcNow },
            new SemanticSearchResult { Id = Guid.NewGuid(), SearchRequestId = requestA, DocumentTitle = "c", Score = 0.8, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new SemanticSearchResultRepository(context);
        var result = await repo.GetByMinScoreAsync(requestA, 0.75, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Score.Should().Be(0.95);
        result[1].Score.Should().Be(0.8);
    }
}

public class AIRoutingPolicyRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_ReturnsPolicy()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var policy = new AIRoutingPolicy { Id = Guid.NewGuid(), Name = "P", CreatedAt = DateTime.UtcNow };
        context.AIRoutingPolicies.Add(policy);
        await context.SaveChangesAsync();

        var repo = new AIRoutingPolicyRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(policy.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("P");
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIRoutingPolicies.AddRange(
            new AIRoutingPolicy { Id = Guid.NewGuid(), Name = "a", Status = RoutingStatus.Active, CreatedAt = DateTime.UtcNow },
            new AIRoutingPolicy { Id = Guid.NewGuid(), Name = "b", Status = RoutingStatus.Inactive, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIRoutingPolicyRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStrategyAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIRoutingPolicies.AddRange(
            new AIRoutingPolicy { Id = Guid.NewGuid(), Name = "a", Strategy = RoutingStrategy.LeastLoaded, CreatedAt = DateTime.UtcNow },
            new AIRoutingPolicy { Id = Guid.NewGuid(), Name = "b", Strategy = RoutingStrategy.RoundRobin, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIRoutingPolicyRepository(context);
        var result = await repo.GetByStrategyAsync("LeastLoaded", CancellationToken.None);

        result.Should().ContainSingle();
    }
}
