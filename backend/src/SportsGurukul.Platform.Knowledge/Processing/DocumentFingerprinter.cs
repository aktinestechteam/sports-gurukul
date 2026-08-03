using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed partial class DocumentFingerprinter : IDocumentFingerprinter
{
    public string Algorithm => "sha256-normalized-v1";

    public DocumentFingerprint Compute(string normalizedText)
    {
        var canonical = Canonicalize(normalizedText);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return new DocumentFingerprint(Algorithm, Convert.ToHexString(hash).ToLowerInvariant());
    }

    internal static string Canonicalize(string normalizedText)
    {
        if (string.IsNullOrEmpty(normalizedText))
        {
            return string.Empty;
        }

        var sample = normalizedText.Length <= 32_768 ? normalizedText : normalizedText[..32_768];
        var collapsed = WhitespaceRegex().Replace(sample, " ").Trim().ToLowerInvariant();
        return collapsed;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
