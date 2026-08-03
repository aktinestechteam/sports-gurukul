using System.Text;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class DeterministicEmbeddingProvider : IEmbeddingProvider
{
    public string Name => "deterministic";
    public int Dimensions { get; }

    private readonly int _dimensions;

    public DeterministicEmbeddingProvider(int dimensions = 384)
    {
        _dimensions = Math.Max(16, dimensions);
        Dimensions = _dimensions;
    }

    public Task<EmbeddingVector> EmbedAsync(string text, CancellationToken ct = default) =>
        Task.FromResult(Embed(text));

    public Task<IReadOnlyList<EmbeddingVector>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        var results = new List<EmbeddingVector>(texts.Count);
        foreach (var text in texts)
        {
            ct.ThrowIfCancellationRequested();
            results.Add(Embed(text));
        }

        return Task.FromResult<IReadOnlyList<EmbeddingVector>>(results);
    }

    public Task<bool> IsHealthyAsync(CancellationToken ct = default) => Task.FromResult(true);

    private EmbeddingVector Embed(string text)
    {
        var vector = new float[_dimensions];
        if (string.IsNullOrWhiteSpace(text))
        {
            return new EmbeddingVector(vector, _dimensions);
        }

        var normalized = text.ToLowerInvariant();
        var tokens = new List<string>();
        var current = new StringBuilder();

        foreach (var c in normalized)
        {
            if (char.IsLetterOrDigit(c))
            {
                current.Append(c);
            }
            else if (current.Length > 0)
            {
                tokens.Add(current.ToString());
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        foreach (var token in tokens)
        {
            AddTokenSignal(vector, token);
        }

        foreach (var token in tokens)
        {
            if (token.Length >= 4)
            {
                for (var i = 0; i <= token.Length - 3; i++)
                {
                    AddTokenSignal(vector, token.Substring(i, 3));
                }
            }
        }

        L2Normalize(vector);
        return new EmbeddingVector(vector, _dimensions);
    }

    private void AddTokenSignal(float[] vector, string token)
    {
        var h1 = Fnv1a(token);
        var h2 = Fnv1a(token, salt: "sg");
        var index = (int)(h1 % (ulong)_dimensions);
        var sign = (h2 & 1) == 0 ? 1f : -1f;
        vector[index] += sign;
    }

    private static ulong Fnv1a(string value, string? salt = null)
    {
        const ulong offsetBasis = 14695981039346656037UL;
        const ulong prime = 1099511628211UL;
        var hash = offsetBasis;
        foreach (var c in value)
        {
            hash ^= c;
            hash *= prime;
        }

        if (salt != null)
        {
            foreach (var c in salt)
            {
                hash ^= c;
                hash *= prime;
            }
        }

        return hash;
    }

    private static void L2Normalize(float[] vector)
    {
        double sum = 0;
        foreach (var v in vector)
        {
            sum += v * v;
        }

        if (sum == 0)
        {
            return;
        }

        var norm = Math.Sqrt(sum);
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)(vector[i] / norm);
        }
    }
}
