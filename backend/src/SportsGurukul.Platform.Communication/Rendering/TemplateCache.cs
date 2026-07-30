using System.Collections.Concurrent;

namespace SportsGurukul.Platform.Communication.Rendering;

public class TemplateCache
{
    private readonly ConcurrentDictionary<string, CachedTemplate> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly int _maxSize;

    public TemplateCache(int maxSize = 500)
    {
        _maxSize = maxSize;
    }

    public bool TryGet(string key, out string compiledTemplate)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            if (!cached.IsExpired)
            {
                compiledTemplate = cached.Template;
                return true;
            }

            _cache.TryRemove(key, out _);
        }

        compiledTemplate = null!;
        return false;
    }

    public void Set(string key, string compiledTemplate, TimeSpan? expiry = null)
    {
        if (_cache.Count >= _maxSize)
        {
            var oldestKey = _cache
                .OrderBy(kvp => kvp.Value.CachedAt)
                .FirstOrDefault().Key;

            if (oldestKey is not null)
                _cache.TryRemove(oldestKey, out _);
        }

        _cache[key] = new CachedTemplate
        {
            Template = compiledTemplate,
            CachedAt = DateTime.UtcNow,
            ExpiresAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null
        };
    }

    public void Invalidate(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void InvalidateAll()
    {
        _cache.Clear();
    }

    private class CachedTemplate
    {
        public string Template { get; set; } = string.Empty;
        public DateTime CachedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
    }
}
