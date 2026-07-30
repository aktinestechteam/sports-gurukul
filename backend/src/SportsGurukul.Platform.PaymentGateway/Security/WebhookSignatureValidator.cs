using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class WebhookSignatureValidator : IPaymentSignatureValidator
{
    private readonly ILogger<WebhookSignatureValidator> _logger;

    public WebhookSignatureValidator(ILogger<WebhookSignatureValidator> logger)
    {
        _logger = logger;
    }

    public bool Validate(string payload, string signature, string secret, string provider)
    {
        if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(secret))
            return false;

        try
        {
            var expected = provider.ToLowerInvariant() switch
            {
                "razorpay" => GenerateHmacSha256(payload, secret),
                "stripe" => GenerateHmacSha256(payload, secret),
                "cashfree" => GenerateHmacSha256(payload, secret),
                "payu" => GenerateHashMd5(payload, secret),
                "paypal" => GenerateHmacSha256(payload, secret),
                _ => GenerateHmacSha256(payload, secret)
            };

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(signature));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signature validation failed for provider {Provider}", provider);
            return false;
        }
    }

    public string Generate(string payload, string secret, string provider)
    {
        return provider.ToLowerInvariant() switch
        {
            "razorpay" => GenerateHmacSha256(payload, secret),
            "stripe" => GenerateHmacSha256(payload, secret),
            "cashfree" => GenerateHmacSha256(payload, secret),
            "payu" => GenerateHashMd5(payload, secret),
            "paypal" => GenerateHmacSha256(payload, secret),
            _ => GenerateHmacSha256(payload, secret)
        };
    }

    public bool ValidateWebhook(WebhookPayload payload, GatewayConfig config)
    {
        if (!ValidateTimestamp(payload.WebhookTimestamp))
        {
            _logger.LogWarning("Webhook timestamp validation failed for {WebhookId}", payload.WebhookId);
            return false;
        }

        var secret = config.WebhookSecret ?? config.ApiSecret;
        if (string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning("No webhook secret configured for provider");
            return false;
        }

        return Validate(payload.RawBody, payload.Signature, secret, config.Provider);
    }

    public bool ValidateTimestamp(DateTime? webhookTimestamp, int maxAgeMinutes = 5)
    {
        if (webhookTimestamp is null) return false;
        return (DateTime.UtcNow - webhookTimestamp.Value).TotalMinutes <= maxAgeMinutes;
    }

    private static string GenerateHmacSha256(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GenerateHashMd5(string payload, string secret)
    {
        var hashInput = $"{payload}|{secret}";
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(hashInput));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
