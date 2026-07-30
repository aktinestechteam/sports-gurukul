using System.Security.Cryptography;
using System.Text;

namespace SportsGurukul.Platform.Communication.Webhook;

public class WebhookSignatureValidator
{
    public string GenerateSignature(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool ValidateSignature(string payload, string signature, string secret)
    {
        var expectedSignature = GenerateSignature(payload, secret);
        return string.Equals(expectedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    public bool ValidateTimestamp(string? timestampHeader, int maxAgeMinutes = 5)
    {
        if (string.IsNullOrEmpty(timestampHeader))
            return false;

        if (long.TryParse(timestampHeader, out var unixTimestamp))
        {
            var timestamp = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            return (DateTimeOffset.UtcNow - timestamp).TotalMinutes <= maxAgeMinutes;
        }

        return false;
    }

    public (bool IsValid, string? Reason) ValidateWebhookRequest(
        string body,
        string? signatureHeader,
        string? timestampHeader,
        string secret,
        int maxAgeMinutes = 5)
    {
        if (string.IsNullOrEmpty(signatureHeader))
            return (false, "Missing signature header");

        if (!ValidateTimestamp(timestampHeader, maxAgeMinutes))
            return (false, "Invalid or expired timestamp");

        if (!ValidateSignature(body, signatureHeader, secret))
            return (false, "Invalid signature");

        return (true, null);
    }
}
