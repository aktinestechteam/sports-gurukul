using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class SignatureValidationEdgeCaseTests
{
    private readonly IPaymentSignatureValidator _validator;

    public SignatureValidationEdgeCaseTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
    }

    [Fact]
    public void Validate_WithEmptySignature_ShouldFail()
    {
        const string payload = "test_payload";

        var isValid = _validator.Validate(payload, "", "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithWhitespaceSignature_ShouldFail()
    {
        const string payload = "test_payload";

        var isValid = _validator.Validate(payload, "   ", "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithMalformedSignature_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var malformedSignature = signature[..^4] + "xxxx";

        var isValid = _validator.Validate(payload, malformedSignature, "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithSignatureExtraCharacters_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var extendedSignature = signature + "extra";

        var isValid = _validator.Validate(payload, extendedSignature, "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithSignatureTruncated_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var truncatedSignature = signature[..^4];

        var isValid = _validator.Validate(payload, truncatedSignature, "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithUppercaseSignature_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var upperSignature = signature.ToUpperInvariant();

        var isValid = _validator.Validate(payload, upperSignature, "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyPayload_ShouldFail()
    {
        var isValid = _validator.Validate("", "signature", "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NullPayload_ShouldFail()
    {
        var isValid = _validator.Validate(null!, "signature", "secret", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptySecret_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var isValid = _validator.Validate(payload, signature, "", "razorpay");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NullSecret_ShouldFail()
    {
        const string payload = "test_payload";
        var signature = _validator.Generate(payload, "secret", "razorpay");

        var isValid = _validator.Validate(payload, signature, null!, "razorpay");

        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("razorpay")]
    [InlineData("stripe")]
    [InlineData("cashfree")]
    [InlineData("paypal")]
    public void HmacSha256_Providers_GenerateValidSignatures(string provider)
    {
        const string payload = "test_hmac_payload";
        const string secret = "hmac_secret_key";

        var signature = _validator.Generate(payload, secret, provider);

        signature.Should().NotBeNullOrEmpty();
        var isValid = _validator.Validate(payload, signature, secret, provider);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void PayU_Md5Algorithm_GeneratesValidSignature()
    {
        const string payload = "payu_data";
        const string secret = "payu_secret";

        var signature = _validator.Generate(payload, secret, "payu");

        signature.Should().NotBeNullOrEmpty();
        var isValid = _validator.Validate(payload, signature, secret, "payu");
        isValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DifferentProviders_UseDifferentAlgorithms()
    {
        const string payload = "cross_provider_test";
        const string secret = "shared_secret";

        var razorpaySig = _validator.Generate(payload, secret, "razorpay");
        var payuSig = _validator.Generate(payload, secret, "payu");

        var razorpayValidatesWithPayu = _validator.Validate(payload, razorpaySig, secret, "payu");
        var payuValidatesWithRazorpay = _validator.Validate(payload, payuSig, secret, "razorpay");

        razorpayValidatesWithPayu.Should().BeFalse();
        payuValidatesWithRazorpay.Should().BeFalse();
    }

    [Fact]
    public void ValidateWebhook_WithEmptyRawBody_ShouldFail()
    {
        var payload = new WebhookPayload
        {
            RawBody = "",
            Signature = _validator.Generate("", "whsec_test", "razorpay"),
            WebhookTimestamp = DateTime.UtcNow,
            WebhookId = "wh_empty_body"
        };

        var config = new GatewayConfig { Provider = "razorpay", WebhookSecret = "whsec_test" };

        var isValid = _validator.ValidateWebhook(payload, config);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWebhook_WithNullRawBody_ShouldFail()
    {
        var payload = new WebhookPayload
        {
            RawBody = null!,
            Signature = "sig",
            WebhookTimestamp = DateTime.UtcNow,
            WebhookId = "wh_null_body"
        };

        var config = new GatewayConfig { Provider = "razorpay", WebhookSecret = "whsec_test" };

        var isValid = _validator.ValidateWebhook(payload, config);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWebhook_ApiSecretFallback_WhenWebhookSecretMissing()
    {
        var rawBody = "{\"event\":\"test\"}";
        var payload = new WebhookPayload
        {
            RawBody = rawBody,
            Signature = _validator.Generate(rawBody, "api_secret_fallback", "razorpay"),
            WebhookTimestamp = DateTime.UtcNow,
            WebhookId = "wh_api_secret"
        };

        var config = new GatewayConfig
        {
            Provider = "razorpay",
            WebhookSecret = null,
            ApiSecret = "api_secret_fallback"
        };

        var isValid = _validator.ValidateWebhook(payload, config);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimestamp_ExactBoundary_ShouldSucceed()
    {
        var timestamp = DateTime.UtcNow.AddMinutes(-4.9);

        var isValid = _validator.ValidateTimestamp(timestamp, 5);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimestamp_JustBeyondBoundary_ShouldFail()
    {
        var timestamp = DateTime.UtcNow.AddMinutes(-5).AddSeconds(-1);

        var isValid = _validator.ValidateTimestamp(timestamp, 5);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimestamp_FutureTimestamp_ShouldSucceedWithinLimit()
    {
        var timestamp = DateTime.UtcNow.AddSeconds(30);

        var isValid = _validator.ValidateTimestamp(timestamp, 5);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimestamp_FutureTimestampBeyondLimit_ShouldSucceed()
    {
        var timestamp = DateTime.UtcNow.AddMinutes(10);

        var isValid = _validator.ValidateTimestamp(timestamp, 5);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void Generate_EmptyPayload_ReturnsSignature()
    {
        var signature = _validator.Generate("", "secret", "razorpay");

        signature.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_NullPayload_ThrowsException()
    {
        var act = () => _validator.Generate(null!, "secret", "razorpay");

        act.Should().Throw<Exception>();
    }
}
