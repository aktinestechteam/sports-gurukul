using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentTokenService
{
    Task<PaymentMethodToken> CreateTokenAsync(
        string customerId,
        string gatewayPaymentMethodId,
        string provider,
        CancellationToken cancellationToken = default);

    Task<PaymentMethodToken?> GetTokenAsync(
        string tokenId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentMethodToken>> GetCustomerTokensAsync(
        string customerId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteTokenAsync(
        string tokenId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteCustomerTokensAsync(
        string customerId,
        CancellationToken cancellationToken = default);

    string MaskSensitiveData(string data);
    string EncryptToken(string token);
    string DecryptToken(string encryptedToken);
}
