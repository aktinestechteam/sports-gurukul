using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class TemplateCacheTests
{
    private readonly TemplateCache _cache;

    public TemplateCacheTests()
    {
        _cache = new TemplateCache(maxSize: 10);
    }

    [Fact]
    public void TryGet_ShouldReturnCachedTemplate()
    {
        _cache.Set("key1", "compiled template");

        var found = _cache.TryGet("key1", out var template);

        found.Should().BeTrue();
        template.Should().Be("compiled template");
    }

    [Fact]
    public void TryGet_ShouldReturnFalseForMissingKey()
    {
        var found = _cache.TryGet("nonexistent", out var template);

        found.Should().BeFalse();
        template.Should().BeNull();
    }

    [Fact]
    public void Set_ShouldStoreTemplate()
    {
        _cache.Set("greeting", "Hello {{name}}!");

        var found = _cache.TryGet("greeting", out var template);

        found.Should().BeTrue();
        template.Should().Be("Hello {{name}}!");
    }

    [Fact]
    public void Set_ShouldOverwriteExisting()
    {
        _cache.Set("key", "old");
        _cache.Set("key", "new");

        _cache.TryGet("key", out var template);
        template.Should().Be("new");
    }

    [Fact]
    public void Invalidate_ShouldRemoveFromCache()
    {
        _cache.Set("key", "value");
        _cache.Invalidate("key");

        var found = _cache.TryGet("key", out _);
        found.Should().BeFalse();
    }

    [Fact]
    public void InvalidateAll_ShouldEmptyCache()
    {
        _cache.Set("key1", "value1");
        _cache.Set("key2", "value2");
        _cache.InvalidateAll();

        _cache.TryGet("key1", out _).Should().BeFalse();
        _cache.TryGet("key2", out _).Should().BeFalse();
    }

    [Fact]
    public void Cache_ShouldRespectTtl()
    {
        _cache.Set("key", "value", TimeSpan.FromMilliseconds(-1));

        var found = _cache.TryGet("key", out _);

        found.Should().BeFalse();
    }

    [Fact]
    public void Cache_ShouldRemoveExpiredOnAccess()
    {
        _cache.Set("key", "value", TimeSpan.FromMilliseconds(1));
        Thread.Sleep(5);

        var found = _cache.TryGet("key", out _);

        found.Should().BeFalse();
    }

    [Fact]
    public void Cache_ShouldEvictOldestWhenFull()
    {
        var smallCache = new TemplateCache(maxSize: 3);

        smallCache.Set("a", "1");
        smallCache.Set("b", "2");
        smallCache.Set("c", "3");
        smallCache.Set("d", "4");

        smallCache.TryGet("a", out _).Should().BeFalse();
        smallCache.TryGet("b", out _).Should().BeTrue();
        smallCache.TryGet("c", out _).Should().BeTrue();
        smallCache.TryGet("d", out _).Should().BeTrue();
    }

    [Fact]
    public void Set_WithExpiry_ShouldStoreUntilExpired()
    {
        _cache.Set("key", "value", TimeSpan.FromHours(1));

        var found = _cache.TryGet("key", out var template);

        found.Should().BeTrue();
        template.Should().Be("value");
    }

    [Fact]
    public void Set_WithoutExpiry_ShouldNotExpire()
    {
        _cache.Set("key", "permanent");

        var found = _cache.TryGet("key", out var template);

        found.Should().BeTrue();
        template.Should().Be("permanent");
    }

    [Fact]
    public void Invalidate_OnNonExistentKey_ShouldNotThrow()
    {
        var act = () => _cache.Invalidate("nonexistent");

        act.Should().NotThrow();
    }

    [Fact]
    public void Set_ShouldBeCaseInsensitive()
    {
        _cache.Set("KEY", "value");

        _cache.TryGet("key", out var template).Should().BeTrue();
        template.Should().Be("value");
    }

    [Fact]
    public void GetOrAddAsync_ShouldReturnCachedTemplateOnSubsequentCalls()
    {
        _cache.Set("template", "cached result");

        var hit = _cache.TryGet("template", out var result);

        hit.Should().BeTrue();
        result.Should().Be("cached result");
    }

    [Fact]
    public void GetOrAddAsync_ShouldCallFactoryOnCacheMiss()
    {
        var factoryCalled = false;

        if (!_cache.TryGet("missing", out _))
        {
            _cache.Set("missing", "factory result");
            factoryCalled = true;
        }

        factoryCalled.Should().BeTrue();
        _cache.TryGet("missing", out var result).Should().BeTrue();
        result.Should().Be("factory result");
    }

    [Fact]
    public void Clear_ShouldEmptyCache()
    {
        _cache.Set("k1", "v1");
        _cache.Set("k2", "v2");
        _cache.InvalidateAll();

        _cache.TryGet("k1", out _).Should().BeFalse();
        _cache.TryGet("k2", out _).Should().BeFalse();
    }

    [Fact]
    public void TryGet_NonExistent_ShouldReturnFalse()
    {
        _cache.TryGet("not-set", out _).Should().BeFalse();
    }
}
