using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IChunkingStrategy
{
    ChunkingStrategyType StrategyType { get; }
    Task<List<DocumentChunk>> ChunkAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default);
}

public record ChunkingOptions(
    ChunkingStrategyType Strategy,
    int MaxChunkSize = 512,
    int ChunkOverlap = 64,
    int? MinChunkSize = 100,
    bool IncludeHeadings = true,
    bool PreserveParagraphs = true,
    string? Separators = "\n\n\n\n\n\n\n\n",
    int? ParentChunkSize = 2048,
    int? ChildChunkSize = 256
);

public interface IChunkingStrategyFactory
{
    IChunkingStrategy GetStrategy(ChunkingStrategyType type);
    bool SupportsStrategy(ChunkingStrategyType type);
}
