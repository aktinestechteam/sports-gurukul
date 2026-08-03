using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed partial class PiiDetector : IPiiDetector
{
    public string Name => "RegexPiiDetector";

    private readonly IReadOnlyList<(string Type, Regex Pattern, string RedactedReplacement)> _rules =
    [
        ("Email", EmailRegex(), "<[email redacted]>"),
        ("Phone", PhoneRegex(), "<[phone redacted]>"),
        ("Aadhaar", AadhaarRegex(), "<[aadhaar redacted]>"),
        ("CreditCard", CreditCardRegex(), "<[card redacted]>"),
        ("Pan", PanRegex(), "<[pan redacted]>"),
        ("BankAccount", BankAccountRegex(), "<[bank-account redacted]>"),
        ("IpAddress", IpAddressRegex(), "<[ip redacted]>")
    ];

    public Task<IReadOnlyList<PiiFinding>> DetectAsync(string text, CancellationToken ct = default)
    {
        var candidates = new List<PiiFinding>();
        foreach (var (type, pattern, replacement) in _rules)
        {
            foreach (Match match in pattern.Matches(text))
            {
                if (!IsProbablyGarbage(match.Value))
                {
                    candidates.Add(new PiiFinding(type, match.Value, match.Index, match.Length, replacement));
                }
            }
        }

        var findings = new List<PiiFinding>();
        var coveredUntil = -1;
        foreach (var finding in candidates.OrderBy(f => f.Offset).ThenBy(f => f.Length))
        {
            if (finding.Offset < coveredUntil)
            {
                continue;
            }

            findings.Add(finding);
            coveredUntil = finding.Offset + finding.Length;
        }

        return Task.FromResult<IReadOnlyList<PiiFinding>>(findings);
    }

    public string Redact(string text, IReadOnlyList<PiiFinding> findings)
    {
        if (findings.Count == 0)
        {
            return text;
        }

        var builder = new StringBuilder(text);
        foreach (var finding in findings.OrderByDescending(f => f.Offset))
        {
            if (finding.Offset >= 0 && finding.Length >= 0 && finding.Offset + finding.Length <= builder.Length)
            {
                builder.Remove(finding.Offset, finding.Length);
                builder.Insert(finding.Offset, finding.RedactedReplacement);
            }
        }

        return builder.ToString();
    }

    private static bool IsProbablyGarbage(string value) =>
        value.Length > 64 || value.Count(c => c == '0') == value.Length;

    [GeneratedRegex(@"[\w.+-]+@[\w-]+\.[\w.-]+", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(?<!\d)(?:\+?91[\s-]?)?(?:[6-9]\d{9})(?!\d)")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"\b[2-9]\d{3}[-\s]?\d{4}[-\s]?\d{4}\b")]
    private static partial Regex AadhaarRegex();

    [GeneratedRegex(@"\b(?:\d[ -]*?){13,19}\b")]
    private static partial Regex CreditCardRegex();

    [GeneratedRegex(@"\b[A-Z]{5}[0-9]{4}[A-Z]\b")]
    private static partial Regex PanRegex();

    [GeneratedRegex(@"\b\d{9,18}\b")]
    private static partial Regex BankAccountRegex();

    [GeneratedRegex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b")]
    private static partial Regex IpAddressRegex();
}
