using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class IdempotencyTests
{
    private readonly IdempotencyService _service;

    public IdempotencyTests()
    {
        _service = new IdempotencyService(NullLogger<IdempotencyService>.Instance);
    }

    [Fact]
    public void TrySetResult_NewKey_ShouldReturnTrue()
    {
        var result = _service.TrySetResult("key1", "value1");
        Assert.True(result);
    }

    [Fact]
    public void TrySetResult_DuplicateKey_ShouldReturnFalse()
    {
        _service.TrySetResult("key1", "value1");
        var result = _service.TrySetResult("key1", "value2");
        Assert.False(result);
    }

    [Fact]
    public void TryGetResult_AfterSet_ShouldReturnValue()
    {
        _service.TrySetResult("key1", "test_value");
        var found = _service.TryGetResult("key1", out var value);
        Assert.True(found);
        Assert.Equal("test_value", value);
    }

    [Fact]
    public void TryGetResult_UnsetKey_ShouldReturnFalse()
    {
        var found = _service.TryGetResult("nonexistent", out var value);
        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void IsDuplicate_NewKey_ShouldReturnFalse()
    {
        Assert.False(_service.IsDuplicate("new_key"));
    }

    [Fact]
    public void IsDuplicate_ExistingKey_ShouldReturnTrue()
    {
        _service.TrySetResult("key1", "value");
        Assert.True(_service.IsDuplicate("key1"));
    }

    [Fact]
    public void Evict_ShouldRemoveKey()
    {
        _service.TrySetResult("key1", "value");
        _service.Evict("key1");
        Assert.False(_service.IsDuplicate("key1"));
    }

    [Fact]
    public void EvictExpired_ShouldNotRemoveActiveKeys()
    {
        _service.TrySetResult("active_key", "value");
        _service.EvictExpired();
        Assert.True(_service.IsDuplicate("active_key"));
    }

    [Fact]
    public void TryGetResult_AfterEvict_ShouldReturnFalse()
    {
        _service.TrySetResult("key1", "value");
        _service.Evict("key1");
        var found = _service.TryGetResult("key1", out _);
        Assert.False(found);
    }

    [Fact]
    public void MultipleKeys_ShouldWorkIndependently()
    {
        _service.TrySetResult("key1", "value1");
        _service.TrySetResult("key2", "value2");

        Assert.True(_service.IsDuplicate("key1"));
        Assert.True(_service.IsDuplicate("key2"));

        _service.Evict("key1");
        Assert.False(_service.IsDuplicate("key1"));
        Assert.True(_service.IsDuplicate("key2"));
    }
}
