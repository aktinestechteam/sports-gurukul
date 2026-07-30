using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Caching;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class CacheServiceTests
{
    private readonly FinancialCacheService _cache;

    public CacheServiceTests()
    {
        _cache = new FinancialCacheService(NullLogger<FinancialCacheService>.Instance);
    }

    [Fact]
    public async Task SetAndGet_ReturnsCachedValue()
    {
        var key = "test_key";
        var value = new RevenueKpi { TotalRevenue = 1000 };
        await _cache.SetAsync(key, value);
        var result = await _cache.GetAsync<RevenueKpi>(key);
        Assert.NotNull(result);
        Assert.Equal(1000, result!.TotalRevenue);
    }

    [Fact]
    public async Task Get_NonExistentKey_ReturnsNull()
    {
        var result = await _cache.GetAsync<RevenueKpi>("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public async Task Exists_ExistingKey_ReturnsTrue()
    {
        await _cache.SetAsync("exists_test", new RevenueKpi());
        var exists = await _cache.ExistsAsync("exists_test");
        Assert.True(exists);
    }

    [Fact]
    public async Task Remove_RemovesKey()
    {
        await _cache.SetAsync("remove_test", new RevenueKpi());
        await _cache.RemoveAsync("remove_test");
        var exists = await _cache.ExistsAsync("remove_test");
        Assert.False(exists);
    }

    [Fact]
    public async Task MultipleKeys_WorkIndependently()
    {
        await _cache.SetAsync("key1", new RevenueKpi { TotalRevenue = 100 });
        await _cache.SetAsync("key2", new RevenueKpi { TotalRevenue = 200 });
        var val1 = await _cache.GetAsync<RevenueKpi>("key1");
        var val2 = await _cache.GetAsync<RevenueKpi>("key2");
        Assert.Equal(100, val1!.TotalRevenue);
        Assert.Equal(200, val2!.TotalRevenue);
    }

    [Fact]
    public void BuildKey_ReturnsCorrectFormat()
    {
        var key = _cache.BuildKey(CacheRegion.Dashboard, "monthly");
        Assert.Equal("fin:Dashboard:monthly", key);
    }

    [Fact]
    public async Task Overwrite_UpdatesValue()
    {
        await _cache.SetAsync("overwrite", new RevenueKpi { TotalRevenue = 100 });
        await _cache.SetAsync("overwrite", new RevenueKpi { TotalRevenue = 200 });
        var result = await _cache.GetAsync<RevenueKpi>("overwrite");
        Assert.Equal(200, result!.TotalRevenue);
    }
}
