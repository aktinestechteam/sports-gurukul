using System.Collections.Concurrent;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class EmbeddingCache
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<string, EmbeddingVector> _cache = new(StringComparer.Ordinal);
    private long _version;

    public EmbeddingCache(EmbeddingOptions options)
    {
        _capacity = Math.Max(1, options.CacheCapacity);
        _version = options.CacheEnabled ? 0 : -1;
    }

    public EmbeddingVector? TryGet(string text)
    {
        if (_version < 0)
        {
            return null;
        }

        return _cache.TryGetValue(text, out var vector) ? vector : null;
    }

    public void Set(string text, EmbeddingVector vector)
    {
        if (_version < 0)
        {
            return;
        }

        if (_cache.Count >= _capacity)
        {
            Trim();
        }

        _cache[text] = vector;
    }

    public void Clear()
    {
        _cache.Clear();
    }

    private void Trim()
    {
        foreach (var key in _cache.Keys.Take(_capacity / 4))
        {
            _cache.TryRemove(key, out _);
        }
    }
}
