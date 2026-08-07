using System.Security.Cryptography;
using System.Text;
using SportsGurukul.Platform.Knowledge.Configuration;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Security;

internal sealed class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(KnowledgePlatformOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Security.EncryptionKeyBase64))
        {
            throw new InvalidOperationException(
                "EncryptionService requires 'KnowledgePlatform:Security:EncryptionKeyBase64' " +
                "(a 32-byte key encoded as base64).");
        }

        try
        {
            _key = Convert.FromBase64String(options.Security.EncryptionKeyBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "KnowledgePlatform:Security:EncryptionKeyBase64 is not valid base64.", ex);
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException(
                $"KnowledgePlatform:Security:EncryptionKeyBase64 must decode to exactly 32 bytes (got {_key.Length}).");
        }
    }

    public string Encrypt(string plaintext)
    {
        var ciphertext = EncryptBytes(Encoding.UTF8.GetBytes(plaintext));
        return Convert.ToBase64String(ciphertext);
    }

    public string Decrypt(string ciphertext)
    {
        var plaintext = DecryptBytes(Convert.FromBase64String(ciphertext));
        return Encoding.UTF8.GetString(plaintext);
    }

    public byte[] EncryptBytes(byte[] plaintext)
    {
        var iv = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];
        var ciphertext = new byte[plaintext.Length];

        using (var aes = new AesGcm(_key, 16))
        {
            aes.Encrypt(iv, plaintext, ciphertext, tag);
        }

        var result = new byte[iv.Length + ciphertext.Length + tag.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(ciphertext, 0, result, iv.Length, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length + ciphertext.Length, tag.Length);
        return result;
    }

    public byte[] DecryptBytes(byte[] ciphertext)
    {
        if (ciphertext.Length < 12 + 16)
        {
            throw new InvalidOperationException("Ciphertext is too short.");
        }

        var iv = new byte[12];
        var tag = new byte[16];
        var payload = new byte[ciphertext.Length - iv.Length - tag.Length];

        Buffer.BlockCopy(ciphertext, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(ciphertext, iv.Length, payload, 0, payload.Length);
        Buffer.BlockCopy(ciphertext, iv.Length + payload.Length, tag, 0, tag.Length);

        var plaintext = new byte[payload.Length];
        using (var aes = new AesGcm(_key, 16))
        {
            aes.Decrypt(iv, payload, tag, plaintext);
        }

        return plaintext;
    }

    public static string GenerateKey() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}
