namespace SportsGurukul.Platform.Knowledge.Models;

public record ChunkingOptions(
    ChunkingStrategyType Strategy = ChunkingStrategyType.Recursive,
    int ChunkSize = 512,
    int ChunkOverlap = 64,
    int MinChunkSize = 64,
    bool UseTokenEstimation = false,
    int MaxHeadingDepth = 2,
    int SemanticSentenceWindow = 4,
    float SemanticMergeThreshold = 0.72f,
    int ParentChunkSize = 1024,
    int ChildChunkSize = 256,
    IReadOnlyList<string>? Separators = null)
{
    public IReadOnlyList<string> Separators { get; init; } =
        Separators ?? new[] { "\n\n", "\n", ". ", "! ", "? ", "; ", ", ", " ", "" };
}
