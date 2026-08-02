using FluentAssertions;
using SportsGurukul.Application.Features.KnowledgePlatform.Chunking;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Chunking;

public class ChunkingStrategyTests
{
    private readonly ExtractedDocument _testDocument = new(
        Id: "doc-1",
        FileName: "test.txt",
        Format: DocumentFormat.PlainText,
        Text: "This is the first sentence about sports training. " +
              "The second sentence discusses athlete nutrition. " +
              "Third sentence covers recovery techniques. " +
              "Fourth sentence explains periodization methods. " +
              "Fifth sentence details strength training protocols. " +
              "Sixth sentence talks about flexibility exercises.",
        Title: "Test Document",
        PageCount: 1,
        DetectedLanguage: "en",
        Author: "Test Author",
        CreatedDate: null,
        ModifiedDate: null,
        Metadata: null,
        Images: null,
        Status: ProcessingStatus.Extracted
    );

    [Fact]
    public async Task FixedSizeChunker_SplitsTextIntoFixedSizeChunks()
    {
        var chunker = new FixedSizeChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.FixedSize, MaxChunkSize: 10, ChunkOverlap: 2);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        chunks.Should().NotBeEmpty();
        chunks.Should().OnlyContain(c => c.Strategy == ChunkingStrategyType.FixedSize);
        chunks.Should().OnlyContain(c => c.DocumentId == "doc-1");
        chunks.First().Content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task FixedSizeChunker_RespectsMaxChunkSize()
    {
        var chunker = new FixedSizeChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.FixedSize, MaxChunkSize: 5, ChunkOverlap: 1);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        foreach (var chunk in chunks)
        {
            var wordCount = chunk.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            wordCount.Should().BeLessOrEqualTo(5);
        }
    }

    [Fact]
    public async Task FixedSizeChunker_HasOverlapBetweenChunks()
    {
        var chunker = new FixedSizeChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.FixedSize, MaxChunkSize: 10, ChunkOverlap: 3);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        if (chunks.Count > 1)
        {
            var firstWords = chunks[0].Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var secondWords = chunks[1].Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var overlap = firstWords.Intersect(secondWords);
            overlap.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task HeadingBasedChunker_SplitsByHeadings()
    {
        var doc = _testDocument with
        {
            Text = "# Introduction\nThis is intro content.\n# Methods\nThis is methods content.\n# Results\nThis is results content."
        };
        var chunker = new HeadingBasedChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.HeadingBased);

        var chunks = await chunker.ChunkAsync(doc, options);

        chunks.Should().HaveCount(3);
        chunks[0].Heading.Should().Be("Introduction");
        chunks[1].Heading.Should().Be("Methods");
        chunks[2].Heading.Should().Be("Results");
    }

    [Fact]
    public async Task RecursiveChunker_SplitsHierarchically()
    {
        var chunker = new RecursiveChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.Recursive, MaxChunkSize: 50, ChunkOverlap: 5);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        chunks.Should().NotBeEmpty();
        chunks.Should().OnlyContain(c => c.Strategy == ChunkingStrategyType.Recursive);
    }

    [Fact]
    public async Task SlidingWindowChunker_CreatesWindowsWithStride()
    {
        var chunker = new SlidingWindowChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.SlidingWindow, MaxChunkSize: 10, ChunkOverlap: 3, MinChunkSize: 3);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        chunks.Should().NotBeEmpty();
        chunks.Should().OnlyContain(c => c.Strategy == ChunkingStrategyType.SlidingWindow);
    }

    [Fact]
    public async Task ParentChildChunker_CreatesParentAndChildChunks()
    {
        var chunker = new ParentChildChunker();
        var options = new ChunkingOptions(
            ChunkingStrategyType.ParentChild,
            MaxChunkSize: 100,
            ChunkOverlap: 5,
            ParentChunkSize: 50,
            ChildChunkSize: 15);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        var parents = chunks.Where(c => c.ParentChunkId == null).ToList();
        var children = chunks.Where(c => c.ParentChunkId != null).ToList();

        parents.Should().NotBeEmpty();
        children.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SemanticChunker_GroupsSentences()
    {
        var chunker = new SemanticChunker();
        var options = new ChunkingOptions(ChunkingStrategyType.Semantic, MaxChunkSize: 20);

        var chunks = await chunker.ChunkAsync(_testDocument, options);

        chunks.Should().NotBeEmpty();
        chunks.Should().OnlyContain(c => c.Strategy == ChunkingStrategyType.Semantic);
    }

    [Fact]
    public async Task ChunkingStrategyFactory_ReturnsRegisteredStrategy()
    {
        var strategies = new IChunkingStrategy[]
        {
            new FixedSizeChunker(),
            new HeadingBasedChunker(),
            new RecursiveChunker()
        };
        var factory = new ChunkingStrategyFactory(strategies);

        var fixedStrategy = factory.GetStrategy(ChunkingStrategyType.FixedSize);
        var headingStrategy = factory.GetStrategy(ChunkingStrategyType.HeadingBased);

        fixedStrategy.Should().BeOfType<FixedSizeChunker>();
        headingStrategy.Should().BeOfType<HeadingBasedChunker>();
        factory.SupportsStrategy(ChunkingStrategyType.FixedSize).Should().BeTrue();
    }

    [Fact]
    public void ChunkingStrategyFactory_ThrowsForUnregisteredStrategy()
    {
        var factory = new ChunkingStrategyFactory(Array.Empty<IChunkingStrategy>());

        Action act = () => factory.GetStrategy(ChunkingStrategyType.Semantic);
        act.Should().Throw<NotSupportedException>();
    }
}
