using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentSignatureValidator
{
    bool Validate(string payload, string signature, string secret, string provider);
    string Generate(string payload, string secret, string provider);
    bool ValidateWebhook(WebhookPayload payload, GatewayConfig config);
    bool ValidateTimestamp(DateTime? webhookTimestamp, int maxAgeMinutes = 5);
}
