using System.Security.Cryptography;
using System.Text;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class SensitiveDataEncryptor
{
    private readonly byte[] _key;

    public SensitiveDataEncryptor(byte[] encryptionKey)
    {
        _key = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
    }

    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return plaintext;

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var ciphertext = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        var result = new byte[aes.IV.Length + ciphertext.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(ciphertext, 0, result, aes.IV.Length, ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encryptedData)
    {
        if (string.IsNullOrEmpty(encryptedData)) return encryptedData;

        var fullCipher = Convert.FromBase64String(encryptedData);
        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.IV.Length];
        var ciphertext = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, ciphertext, 0, ciphertext.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plaintextBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        return Encoding.UTF8.GetString(plaintextBytes);
    }

    public string Mask(string data, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(data) || data.Length <= visibleChars)
            return data;

        return new string('*', data.Length - visibleChars) + data[^visibleChars..];
    }
}
