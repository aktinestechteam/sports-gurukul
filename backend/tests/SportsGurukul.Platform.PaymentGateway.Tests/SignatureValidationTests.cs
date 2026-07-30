using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class SignatureValidationTests
{
    private readonly IPaymentSignatureValidator _validator;

    public SignatureValidationTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
    }

    [Theory]
    [InlineData("payload123", "secret123", "razorpay")]
    [InlineData("test_data", "test_key", "stripe")]
    [InlineData("webhook_body", "whsec_abc123", "cashfree")]
    public void GenerateAndValidate_ShouldSucceed(string payload, string secret, string provider)
    {
        var signature = _validator.Generate(payload, secret, provider);
        var isValid = _validator.Validate(payload, signature, secret, provider);
        Assert.True(isValid);
    }

    [Fact]
    public void Validate_WithWrongSecret_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "correct_secret", "razorpay");
        var isValid = _validator.Validate(payload, signature, "wrong_secret", "razorpay");
        Assert.False(isValid);
    }

    [Fact]
    public void Validate_WithTamperedPayload_ShouldFail()
    {
        const string payload = "original_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");
        var isValid = _validator.Validate("tampered_payload", signature, "secret", "razorpay");
        Assert.False(isValid);
    }

    [Fact]
    public void Validate_EmptyPayload_ShouldFail()
    {
        var isValid = _validator.Validate("", "signature", "secret", "razorpay");
        Assert.False(isValid);
    }

    [Fact]
    public void Validate_NullPayload_ShouldFail()
    {
        var isValid = _validator.Validate(null!, "signature", "secret", "razorpay");
        Assert.False(isValid);
    }

    [Fact]
    public void PayU_GenerateAndValidate_ShouldSucceed()
    {
        const string payload = "payu_data";
        const string secret = "payu_secret";
        var signature = _validator.Generate(payload, secret, "payu");
        var isValid = _validator.Validate(payload, signature, secret, "payu");
        Assert.True(isValid);
    }

    [Fact]
    public void PayPal_GenerateAndValidate_ShouldSucceed()
    {
        const string payload = "paypal_webhook";
        const string secret = "paypal_secret";
        var signature = _validator.Generate(payload, secret, "paypal");
        var isValid = _validator.Validate(payload, signature, secret, "paypal");
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateWebhook_WithValidTimestamp_ShouldSucceed()
    {
        var payload = new WebhookPayload
        {
            RawBody = "{\"event\":\"payment.success\"}",
            Signature = _validator.Generate("{\"event\":\"payment.success\"}", "whsec_test", "razorpay"),
            WebhookTimestamp = DateTime.UtcNow.AddSeconds(-30),
            WebhookId = "wh_001"
        };

        var config = new GatewayConfig
        {
            Provider = "razorpay",
            WebhookSecret = "whsec_test"
        };

        var isValid = _validator.ValidateWebhook(payload, config);
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateWebhook_WithExpiredTimestamp_ShouldFail()
    {
        var payload = new WebhookPayload
        {
            RawBody = "test",
            Signature = "sig",
            WebhookTimestamp = DateTime.UtcNow.AddHours(-1),
            WebhookId = "wh_002"
        };

        var config = new GatewayConfig
        {
            Provider = "razorpay",
            WebhookSecret = "secret"
        };

        var isValid = _validator.ValidateWebhook(payload, config);
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateWebhook_NoSecret_ShouldFail()
    {
        var payload = new WebhookPayload
        {
            RawBody = "test",
            Signature = "sig",
            WebhookTimestamp = DateTime.UtcNow,
            WebhookId = "wh_003"
        };

        var config = new GatewayConfig
        {
            Provider = "razorpay",
            WebhookSecret = null
        };

        var isValid = _validator.ValidateWebhook(payload, config);
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateTimestamp_Null_ShouldFail()
    {
        Assert.False(_validator.ValidateTimestamp(null));
    }

    [Fact]
    public void ValidateTimestamp_WithinLimit_ShouldSucceed()
    {
        Assert.True(_validator.ValidateTimestamp(DateTime.UtcNow.AddSeconds(-30), 5));
    }

    [Fact]
    public void ValidateTimestamp_ExceedsLimit_ShouldFail()
    {
        Assert.False(_validator.ValidateTimestamp(DateTime.UtcNow.AddMinutes(-10), 5));
    }
}
