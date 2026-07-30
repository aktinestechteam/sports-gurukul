using SportsGurukul.Platform.Communication.Webhook;

namespace SportsGurukul.Communication.Infrastructure.Tests.Webhook;

public class WebhookSignatureValidatorTests
{
    private readonly WebhookSignatureValidator _validator = new();

    [Fact]
    public void ValidateSignature_ReturnsTrue_ForValidHMAC()
    {
        var payload = """{"event":"test","data":"value"}""";
        var secret = "my-secret-key";

        var signature = _validator.GenerateSignature(payload, secret);
        var result = _validator.ValidateSignature(payload, signature, secret);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_ForInvalidSignature()
    {
        var payload = """{"event":"test"}""";
        var secret = "my-secret-key";

        var result = _validator.ValidateSignature(payload, "invalid-signature", secret);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_ForTamperedPayload()
    {
        var payload = """{"event":"test"}""";
        var secret = "my-secret-key";

        var signature = _validator.GenerateSignature(payload, secret);

        var tamperedPayload = """{"event":"hacked"}""";
        var result = _validator.ValidateSignature(tamperedPayload, signature, secret);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_HandlesEmptyPayload()
    {
        var payload = "";
        var secret = "secret";

        var signature = _validator.GenerateSignature(payload, secret);
        var result = _validator.ValidateSignature(payload, signature, secret);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_ForNullSignature()
    {
        var payload = """{"event":"test"}""";
        var secret = "secret";

        var result = _validator.ValidateSignature(payload, null!, secret);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_UsesSHA256Algorithm()
    {
        var payload = "test-payload";
        var secret = "test-secret";

        var signature = _validator.GenerateSignature(payload, secret);

        signature.Should().NotBeNullOrEmpty();
        signature.Length.Should().Be(64);
    }

    [Fact]
    public void ValidateSignature_IsCaseInsensitive()
    {
        var payload = "test";
        var secret = "secret";

        var signature = _validator.GenerateSignature(payload, secret);
        var upperSignature = signature.ToUpperInvariant();
        var result = _validator.ValidateSignature(payload, upperSignature, secret);

        result.Should().BeTrue();
    }

    [Fact]
    public void GenerateSignature_ReturnsConsistentResults()
    {
        var payload = "consistent-payload";
        var secret = "consistent-secret";

        var sig1 = _validator.GenerateSignature(payload, secret);
        var sig2 = _validator.GenerateSignature(payload, secret);

        sig1.Should().Be(sig2);
    }

    [Fact]
    public void GenerateSignature_DifferentSecrets_ProduceDifferentSignatures()
    {
        var payload = "test";

        var sig1 = _validator.GenerateSignature(payload, "secret1");
        var sig2 = _validator.GenerateSignature(payload, "secret2");

        sig1.Should().NotBe(sig2);
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_ForWrongSecret()
    {
        var payload = "test";
        var signature = _validator.GenerateSignature(payload, "real-secret");
        var result = _validator.ValidateSignature(payload, signature, "wrong-secret");

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimestamp_ReturnsTrue_ForValidTimestamp()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var result = _validator.ValidateTimestamp(timestamp, 5);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimestamp_ReturnsFalse_ForExpiredTimestamp()
    {
        var timestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds().ToString();

        var result = _validator.ValidateTimestamp(timestamp, 5);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimestamp_ReturnsFalse_ForNullTimestamp()
    {
        var result = _validator.ValidateTimestamp(null, 5);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimestamp_ReturnsFalse_ForEmptyTimestamp()
    {
        var result = _validator.ValidateTimestamp(string.Empty, 5);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimestamp_ReturnsFalse_ForInvalidFormat()
    {
        var result = _validator.ValidateTimestamp("not-a-number", 5);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateWebhookRequest_ReturnsValid_WhenAllChecksPass()
    {
        var payload = "test-body";
        var secret = "webhook-secret";
        var signature = _validator.GenerateSignature(payload, secret);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var (isValid, reason) = _validator.ValidateWebhookRequest(payload, signature, timestamp, secret);

        isValid.Should().BeTrue();
        reason.Should().BeNull();
    }

    [Fact]
    public void ValidateWebhookRequest_ReturnsInvalid_WhenSignatureMissing()
    {
        var (isValid, reason) = _validator.ValidateWebhookRequest("body", null, "12345", "secret");

        isValid.Should().BeFalse();
        reason.Should().Be("Missing signature header");
    }

    [Fact]
    public void ValidateWebhookRequest_ReturnsInvalid_WhenTimestampExpired()
    {
        var payload = "body";
        var secret = "secret";
        var signature = _validator.GenerateSignature(payload, secret);
        var timestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds().ToString();

        var (isValid, reason) = _validator.ValidateWebhookRequest(payload, signature, timestamp, secret, 5);

        isValid.Should().BeFalse();
        reason.Should().Be("Invalid or expired timestamp");
    }

    [Fact]
    public void ValidateWebhookRequest_ReturnsInvalid_WhenSignatureInvalid()
    {
        var payload = "body";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var (isValid, reason) = _validator.ValidateWebhookRequest(payload, "bad-sig", timestamp, "secret");

        isValid.Should().BeFalse();
        reason.Should().Be("Invalid signature");
    }
}
