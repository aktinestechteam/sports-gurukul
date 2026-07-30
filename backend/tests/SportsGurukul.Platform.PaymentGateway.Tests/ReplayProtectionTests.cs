using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class ReplayProtectionTests
{
    private readonly ReplayProtectionService _service;

    public ReplayProtectionTests()
    {
        _service = new ReplayProtectionService(NullLogger<ReplayProtectionService>.Instance);
    }

    [Fact]
    public void IsReplayAttack_FirstRequest_ShouldReturnFalse()
    {
        var result = _service.IsReplayAttack("wh_001", "nonce_001", DateTime.UtcNow);
        Assert.False(result);
    }

    [Fact]
    public void IsReplayAttack_DuplicateWebhookId_ShouldReturnTrue()
    {
        _service.IsReplayAttack("wh_001", "nonce_001", DateTime.UtcNow);
        var result = _service.IsReplayAttack("wh_001", "nonce_002", DateTime.UtcNow);
        Assert.True(result);
    }

    [Fact]
    public void IsReplayAttack_DuplicateNonce_ShouldReturnTrue()
    {
        _service.IsReplayAttack("", "nonce_001", DateTime.UtcNow);
        var result = _service.IsReplayAttack("", "nonce_001", DateTime.UtcNow);
        Assert.True(result);
    }

    [Fact]
    public void IsReplayAttack_OldTimestamp_ShouldReturnTrue()
    {
        var result = _service.IsReplayAttack("wh_003", "nonce_003", DateTime.UtcNow.AddMinutes(-10));
        Assert.True(result);
    }

    [Fact]
    public void IsReplayAttack_EmptyIds_ShouldReturnFalse()
    {
        var result = _service.IsReplayAttack("", "", DateTime.UtcNow);
        Assert.False(result);
    }

    [Fact]
    public void ValidateNonce_NewNonce_ShouldReturnTrue()
    {
        var result = _service.ValidateNonce("fresh_nonce", DateTime.UtcNow);
        Assert.True(result);
    }

    [Fact]
    public void ValidateNonce_DuplicateNonce_ShouldReturnFalse()
    {
        _service.ValidateNonce("nonce", DateTime.UtcNow);
        var result = _service.ValidateNonce("nonce", DateTime.UtcNow);
        Assert.False(result);
    }

    [Fact]
    public void ValidateNonce_OldTimestamp_ShouldReturnFalse()
    {
        var result = _service.ValidateNonce("old_nonce", DateTime.UtcNow.AddHours(-1));
        Assert.False(result);
    }

    [Fact]
    public void ValidateNonce_EmptyNonce_ShouldReturnFalse()
    {
        var result = _service.ValidateNonce("", DateTime.UtcNow);
        Assert.False(result);
    }
}
