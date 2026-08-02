using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Security;

public partial class DefaultOutputValidator : IOutputValidator
{
    private readonly ILogger<DefaultOutputValidator> _logger;

    public DefaultOutputValidator(ILogger<DefaultOutputValidator>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultOutputValidator>.Instance;
    }

    public Task<OutputValidationResult> ValidateAsync(string output, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(output))
        {
            return Task.FromResult(new OutputValidationResult { IsValid = true });
        }

        var violations = new List<string>();
        var sanitized = output;

        foreach (var (name, regex) in SensitivePatterns())
        {
            if (regex.IsMatch(output))
            {
                violations.Add(name);
                sanitized = regex.Replace(sanitized, name == "EmailAddress" ? "[email redacted]" : "[redacted]");
            }
        }

        if (violations.Count > 0)
        {
            _logger.LogWarning("Output validation flagged sensitive data: {Violations}", string.Join(", ", violations));
        }

        return Task.FromResult(new OutputValidationResult
        {
            IsValid = violations.Count == 0,
            Violations = violations,
            SanitizedOutput = sanitized
        });
    }

    private static (string Name, Regex Regex)[] SensitivePatterns()
    {
        return
        [
            ("SsnNumber", SsnRegex()),
            ("CreditCardNumber", CreditCardRegex()),
            ("EmailAddress", EmailRegex()),
            ("BankAccount", BankAccountRegex())
        ];
    }

    [GeneratedRegex(@"\b\d{3}-\d{2}-\d{4}\b")]
    private static partial Regex SsnRegex();

    [GeneratedRegex(@"\b(?:\d[ -]?){13,16}\b")]
    private static partial Regex CreditCardRegex();

    [GeneratedRegex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"\b\d{9,18}\b")]
    private static partial Regex BankAccountRegex();
}
