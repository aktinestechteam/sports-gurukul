using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class IndexingTests
{
    [Fact]
    public async Task KnowledgeIndexService_IndexLifecycle_CreateArchiveRestoreDelete()
    {
        using var provider = TestHarness.CreateProvider();
        var indexService = provider.GetRequiredService<IKnowledgeIndexService>();
        var indexStore = provider.GetRequiredService<IKnowledgeIndexStore>();

        var created = await indexService.CreateIndexAsync("coaching", "tenant-1");
        Assert.Equal(IndexLifecycleState.Active, created.State);
        Assert.Equal(0, created.DocumentCount);

        await indexService.ArchiveIndexAsync("coaching", "tenant-1");
        Assert.Equal(IndexLifecycleState.Archived, (await indexStore.GetIndexAsync("coaching", "tenant-1"))!.State);

        await indexService.RestoreIndexAsync("coaching", "tenant-1");
        Assert.Equal(IndexLifecycleState.Active, (await indexStore.GetIndexAsync("coaching", "tenant-1"))!.State);

        await indexService.DeleteIndexAsync("coaching", "tenant-1");
        Assert.Null(await indexStore.GetIndexAsync("coaching", "tenant-1"));
    }

    [Fact]
    public async Task IncrementalIndex_TracksAddedUpdatedAndSkipped()
    {
        using var provider = TestHarness.CreateProvider();
        var indexService = provider.GetRequiredService<IKnowledgeIndexService>();
        var id = Guid.NewGuid();

        var added = await indexService.IncrementalIndexAsync(
            new[] { TestHarness.Document("Doc", "first version about cricket batting", "t1", "sports", id: id) },
            "sports",
            "t1");
        Assert.Equal(1, added.AddedDocuments);

        var skipped = await indexService.IncrementalIndexAsync(
            new[] { TestHarness.Document("Doc", "first version about cricket batting", "t1", "sports", id: id) },
            "sports",
            "t1");
        Assert.Equal(1, skipped.SkippedDuplicates);

        var updated = await indexService.IncrementalIndexAsync(
            new[] { TestHarness.Document("Doc", "completely different second version now", "t1", "sports", id: id) },
            "sports",
            "t1");
        Assert.Equal(1, updated.UpdatedDocuments);
    }

    [Fact]
    public async Task IngestAsync_ReturnsDuplicateSkipped_ForIdenticalContent()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var document = TestHarness.Document("Doc", "duplicate content here", "t1", "sports");

        var first = await ingestion.IngestAsync(document);
        var second = await ingestion.IngestAsync(document);

        Assert.Equal(DocumentIngestionState.Indexed, first.State);
        Assert.Equal(DocumentIngestionState.DuplicateSkipped, second.State);
        Assert.True(first.ChunkCount > 0);
        Assert.Equal(0, second.ChunkCount);
    }

    [Fact]
    public async Task DeleteAsync_RemovesChunksAndRecord()
    {
        using var provider = TestHarness.CreateProvider();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();
        var indexStore = provider.GetRequiredService<IKnowledgeIndexStore>();
        var vectorStore = provider.GetRequiredService<IVectorStoreFactory>().GetStore();
        var document = TestHarness.Document("Doc", "content to delete later", "t1", "sports");

        await ingestion.IngestAsync(document);
        Assert.True(await vectorStore.CountAsync("sports") > 0);

        var deleted = await ingestion.DeleteAsync(document.Id);

        Assert.True(deleted);
        Assert.Equal(0, await vectorStore.CountAsync("sports"));
        Assert.Null(await indexStore.GetDocumentAsync(document.Id));
    }

    [Fact]
    public async Task IngestAsync_IntoArchivedIndex_Fails()
    {
        using var provider = TestHarness.CreateProvider();
        var indexService = provider.GetRequiredService<IKnowledgeIndexService>();
        var ingestion = provider.GetRequiredService<IKnowledgeIngestionService>();

        await indexService.CreateIndexAsync("coaching", "t1");
        await indexService.ArchiveIndexAsync("coaching", "t1");

        var report = await ingestion.IngestAsync(TestHarness.Document("Doc", "text", "t1", "coaching"));

        Assert.Equal(DocumentIngestionState.Failed, report.State);
        Assert.NotNull(report.Error);
    }

    [Fact]
    public async Task ReindexAsync_RecreatesChunks_AndBumpsVersion()
    {
        using var provider = TestHarness.CreateProvider();
        var indexService = provider.GetRequiredService<IKnowledgeIndexService>();
        var indexStore = provider.GetRequiredService<IKnowledgeIndexStore>();
        var vectorStore = provider.GetRequiredService<IVectorStoreFactory>().GetStore();

        await indexService.CreateIndexAsync("sports", "t1");
        await indexService.IncrementalIndexAsync(
            new[] { TestHarness.Document("Doc", "some indexed content", "t1", "sports") },
            "sports",
            "t1");

        var before = await indexStore.GetIndexAsync("sports", "t1");
        var refreshed = await indexService.ReindexAsync("sports", "t1");

        Assert.Equal(before!.Version + 1, refreshed.Version);
        Assert.True(await vectorStore.CountAsync("sports") > 0);
    }
}
