using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Features.KnowledgePlatform.Chunking;
using SportsGurukul.Application.Features.KnowledgePlatform.Embedding;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;
using SportsGurukul.Application.Features.KnowledgePlatform.Parsers;
using SportsGurukul.Application.Features.KnowledgePlatform.Services;
using SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Performance;

public class IngestionPerformanceTests
{
    [Fact]
    public async Task BatchIngestion_ProcessesMultipleDocuments()
    {
        var parserFactory = CreateParserFactory();
        var chunkingFactory = CreateChunkingFactory();
        var embeddingFactory = CreateEmbeddingFactory();
        var vectorStoreFactory = CreateVectorStoreFactory();

        var documentProcessor = new DocumentProcessingService(parserFactory, NullLogger<DocumentProcessingService>.Instance);
        var chunkingService = new ChunkingService(chunkingFactory, NullLogger<ChunkingService>.Instance);
        var embeddingService = new EmbeddingService(embeddingFactory, NullLogger<EmbeddingService>.Instance);
        var vectorStoreService = new VectorStoreService(vectorStoreFactory, NullLogger<VectorStoreService>.Instance);

        var ingestionService = new KnowledgeIngestionService(
            documentProcessor, chunkingService, embeddingService, vectorStoreService,
            NullLogger<KnowledgeIngestionService>.Instance);

        var documents = Enumerable.Range(1, 5).Select(i => new RawDocument(
            Id: $"perf-doc-{i}",
            FileName: $"doc{i}.txt",
            Format: DocumentFormat.PlainText,
            Content: System.Text.Encoding.UTF8.GetBytes($"This is test document {i}. " + string.Join(" ", Enumerable.Repeat("Performance testing content for benchmarking purposes. ", 50))),
            FileSizeBytes: 1000,
            ContentType: "text/plain",
            SourceUri: null,
            Metadata: null
        )).ToList();

        var sw = Stopwatch.StartNew();

        var result = await ingestionService.IngestDocumentBatchAsync(
            documents,
            new ChunkingOptions(ChunkingStrategyType.FixedSize, MaxChunkSize: 50, ChunkOverlap: 10),
            EmbeddingProviderType.OpenAI,
            "Qdrant",
            CancellationToken.None);

        sw.Stop();

        result.Should().Be(ProcessingStatus.Indexed);
        sw.ElapsedMilliseconds.Should().BeLessThan(30000);
    }

    [Fact]
    public async Task EmbeddingService_ProcessesBatch()
    {
        var factory = CreateEmbeddingFactory();
        var embeddingService = new EmbeddingService(factory, NullLogger<EmbeddingService>.Instance);

        var chunks = Enumerable.Range(1, 20).Select(i => new DocumentChunk(
            Id: $"chunk-{i}",
            DocumentId: "perf-doc",
            ChunkIndex: i,
            Content: $"Test content for chunk {i}. " + string.Join(" ", Enumerable.Repeat("word ", 30)),
            TokenCount: 35,
            CharacterCount: 200,
            Heading: null,
            PageNumber: null,
            ParentChunkId: null,
            Metadata: null,
            Strategy: ChunkingStrategyType.FixedSize
        )).ToList();

        var sw = Stopwatch.StartNew();

        var result = await embeddingService.GenerateEmbeddingsAsync(
            chunks, EmbeddingProviderType.OpenAI, "test-model", CancellationToken.None);

        sw.Stop();

        result.Should().HaveCount(20);
        sw.ElapsedMilliseconds.Should().BeLessThan(10000);
    }

    private static IDocumentParserFactory CreateParserFactory()
    {
        return new DocumentParserFactory([new TxtParser()]);
    }

    private static IChunkingStrategyFactory CreateChunkingFactory()
    {
        return new ChunkingStrategyFactory([new FixedSizeChunker()]);
    }

    private static IEmbeddingProviderFactory CreateEmbeddingFactory()
    {
        return new EmbeddingProviderFactory([new OpenAIEmbeddingProvider()]);
    }

    private static IVectorStoreFactory CreateVectorStoreFactory()
    {
        return new VectorStoreFactory([new QdrantVectorStore()]);
    }
}
