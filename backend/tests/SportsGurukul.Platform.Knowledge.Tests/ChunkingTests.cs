using SportsGurukul.Platform.Knowledge.Chunking;
using SportsGurukul.Platform.Knowledge.Models;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class ChunkingTests
{
    [Fact]
    public void RecursiveChunker_Splits_LargeText_IntoMultipleChunks()
    {
        var service = new ChunkingService(new ChunkingStrategyRegistry());
        var text = string.Join(' ', Enumerable.Repeat("cricket batting technique and bowling drills", 30));
        var document = TestHarness.Document("Manual", text, "t1", "sports");

        var chunks = service.Chunk(document, text, new ChunkingOptions(
            Strategy: ChunkingStrategyType.Recursive,
            ChunkSize: 100,
            MinChunkSize: 1));

        Assert.True(chunks.Count > 1);
        Assert.All(chunks, c => Assert.False(string.IsNullOrWhiteSpace(c.Text)));
        Assert.Equal(document.Id, chunks[0].DocumentId);
    }

    [Fact]
    public void HeadingBasedChunker_ProducesChunks_PerHeading()
    {
        var service = new ChunkingService(new ChunkingStrategyRegistry());
        var text =
            "# Introduction\nFirst paragraph about cricket.\n" +
            "## Batting\nDetails about batting technique.\n" +
            "# Bowling\nBowling tips and tricks.";
        var document = TestHarness.Document("Guide", text, "t1", "sports");

        var chunks = service.Chunk(document, text, new ChunkingOptions(
            Strategy: ChunkingStrategyType.HeadingBased,
            MinChunkSize: 1));

        Assert.Equal(3, chunks.Count);
        Assert.Equal("Introduction", chunks[0].Heading);
        Assert.Equal("Batting", chunks[1].Heading);
        Assert.Equal("Bowling", chunks[2].Heading);
        Assert.Equal(0, chunks[0].Order);
        Assert.Equal(2, chunks[2].Order);
    }

    [Fact]
    public void ChunkingService_DefaultStrategy_IsRecursive()
    {
        var service = new ChunkingService(new ChunkingStrategyRegistry());
        var text = "A short body of text that fits within a single chunk easily.";
        var document = TestHarness.Document("Short", text, "t1", "sports");

        var chunks = service.Chunk(document, text, new ChunkingOptions(MinChunkSize: 1));

        Assert.Single(chunks);
        Assert.Contains(text, chunks[0].Text);
    }

    [Fact]
    public void FixedSizeChunker_ProducesChunks()
    {
        var service = new ChunkingService(new ChunkingStrategyRegistry());
        var text = string.Join(' ', Enumerable.Repeat("word", 200));
        var document = TestHarness.Document("Doc", text, "t1", "sports");

        var fixedChunks = service.Chunk(document, text, new ChunkingOptions(
            Strategy: ChunkingStrategyType.FixedSize,
            ChunkSize: 120,
            MinChunkSize: 1));

        Assert.True(fixedChunks.Count > 1);
        Assert.All(fixedChunks, c => Assert.False(string.IsNullOrWhiteSpace(c.Text)));
    }
}
