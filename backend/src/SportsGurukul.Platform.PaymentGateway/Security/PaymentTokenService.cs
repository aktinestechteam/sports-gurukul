using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class PaymentTokenService : IPaymentTokenService
{
    private readonly ConcurrentDictionary<string, PaymentMethodToken> _tokenStore = new();
    private readonly ConcurrentDictionary<string, List<string>> _customerTokens = new();
    private readonly ILogger<PaymentTokenService> _logger;
    private readonly byte[] _encryptionKey;

    public PaymentTokenService(ILogger<PaymentTokenService> logger)
    {
        _logger = logger;
        _encryptionKey = GenerateEncryptionKey();
    }

    public Task<PaymentMethodToken> CreateTokenAsync(
        string customerId,
        string gatewayPaymentMethodId,
        string provider,
        CancellationToken cancellationToken = default)
    {
        var tokenId = $"tok_{Guid.NewGuid():N}";
        var token = new PaymentMethodToken
        {
            TokenId = tokenId,
            GatewayTokenId = gatewayPaymentMethodId,
            Provider = provider,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        _tokenStore.TryAdd(tokenId, token);

        _customerTokens.AddOrUpdate(
            customerId,
            _ => [tokenId],
            (_, list) =>
            {
                list.Add(tokenId);
                return list;
            });

        _logger.LogInformation("Created payment token {TokenId} for customer {CustomerId}", tokenId, customerId);
        return Task.FromResult(token);
    }

    public Task<PaymentMethodToken?> GetTokenAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        _tokenStore.TryGetValue(tokenId, out var token);
        return Task.FromResult(token);
    }

    public Task<IReadOnlyList<PaymentMethodToken>> GetCustomerTokensAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (_customerTokens.TryGetValue(customerId, out var tokenIds))
        {
            var tokens = tokenIds
                .Select(id => _tokenStore.TryGetValue(id, out var t) ? t : null)
                .Where(t => t is not null)
                .Cast<PaymentMethodToken>()
                .ToList();

            return Task.FromResult<IReadOnlyList<PaymentMethodToken>>(tokens);
        }

        return Task.FromResult<IReadOnlyList<PaymentMethodToken>>([]);
    }

    public Task<bool> DeleteTokenAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        if (_tokenStore.TryRemove(tokenId, out var token))
        {
            if (token.CustomerId is not null && _customerTokens.TryGetValue(token.CustomerId, out var list))
            {
                list.Remove(tokenId);
            }
            _logger.LogInformation("Deleted payment token {TokenId}", tokenId);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DeleteCustomerTokensAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (_customerTokens.TryRemove(customerId, out var tokenIds))
        {
            foreach (var id in tokenIds)
            {
                _tokenStore.TryRemove(id, out _);
            }
            _logger.LogInformation("Deleted {Count} payment tokens for customer {CustomerId}", tokenIds.Count, customerId);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public string MaskSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data) || data.Length < 4)
            return data;

        return data.Length switch
        {
            <= 4 => new string('*', data.Length - 1) + data[^1],
            _ => new string('*', data.Length - 4) + data[^4..]
        };
    }

    public string EncryptToken(string token)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(token);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var ciphertext = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        var result = new byte[aes.IV.Length + ciphertext.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(ciphertext, 0, result, aes.IV.Length, ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public string DecryptToken(string encryptedToken)
    {
        var fullCipher = Convert.FromBase64String(encryptedToken);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        var iv = new byte[aes.IV.Length];
        var ciphertext = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, ciphertext, 0, ciphertext.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plaintextBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        return Encoding.UTF8.GetString(plaintextBytes);
    }

    private static byte[] GenerateEncryptionKey()
    {
        var key = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return key;
    }
}
