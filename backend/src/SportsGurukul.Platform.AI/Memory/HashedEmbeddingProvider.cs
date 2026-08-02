using System.Text.RegularExpressions;
using SportsGurukul.Platform.AI.Interfaces.Memory;

namespace SportsGurukul.Platform.AI.Memory;

public class HashedEmbeddingProvider : IEmbeddingProvider
{
    public const int DefaultDimensions = 64;

    private static readonly Regex WordPattern = new("[a-z0-9']+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly int _dimensions;

    public HashedEmbeddingProvider(int dimensions = DefaultDimensions)
    {
        _dimensions = dimensions > 0 ? dimensions : DefaultDimensions;
    }

    public string Provider => "hashed";

    public Task<IReadOnlyList<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var vector = new float[_dimensions];
        if (string.IsNullOrEmpty(text))
        {
            return Task.FromResult<IReadOnlyList<float>>(vector);
        }

        var normalized = text.ToLowerInvariant();
        foreach (Match match in WordPattern.Matches(normalized))
        {
            var hash = StableHash(match.Value);
            var index = hash % (uint)_dimensions;
            var sign = (hash >> 8) % 2 == 0 ? 1f : -1f;
            vector[index] += sign;
        }

        NormalizeInPlace(vector);
        return Task.FromResult<IReadOnlyList<float>>(vector);
    }

    public Task<int> DimensionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_dimensions);
    }

    private static uint StableHash(string token)
    {
        uint hash = 2166136261;
        foreach (var c in token)
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    private static void NormalizeInPlace(float[] vector)
    {
        double magnitude = 0;
        foreach (var value in vector)
        {
            magnitude += value * value;
        }

        if (magnitude <= 0)
        {
            return;
        }

        var norm = Math.Sqrt(magnitude);
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)(vector[i] / norm);
        }
    }
}
