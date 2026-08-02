namespace SportsGurukul.Platform.AI.Interfaces.Memory;

public interface IEmbeddingProvider
{
    string Provider { get; }

    Task<IReadOnlyList<float>> EmbedAsync(string text, CancellationToken cancellationToken = default);

    Task<int> DimensionsAsync(CancellationToken cancellationToken = default);
}
