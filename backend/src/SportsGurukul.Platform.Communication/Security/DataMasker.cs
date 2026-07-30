using System.Text.RegularExpressions;

namespace SportsGurukul.Platform.Communication.Security;

public class DataMasker
{
    public string MaskEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return string.Empty;

        var atIndex = email.IndexOf('@');
        if (atIndex <= 1)
            return email;

        var localPart = email[..atIndex];
        var domain = email[atIndex..];

        var maskedLocal = localPart.Length <= 2
            ? localPart[..1] + "***"
            : localPart[..1] + "***" + localPart[^1];

        return maskedLocal + domain;
    }

    public string MaskPhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 6)
            return phone ?? string.Empty;

        var visible = phone[^4..];
        var masked = new string('*', phone.Length - 4);
        return masked + visible;
    }

    public string MaskSensitiveValue(string? value, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Length <= visibleChars)
            return new string('*', value.Length);

        var visible = value[^visibleChars..];
        return new string('*', value.Length - visibleChars) + visible;
    }

    public string MaskJsonSensitiveFields(string json, params string[] sensitiveFields)
    {
        if (string.IsNullOrEmpty(json))
            return json;

        var result = json;

        foreach (var field in sensitiveFields)
        {
            var pattern = $"(\"{field}\"\\s*:\\s*\")([^\"]+)\"";
            result = Regex.Replace(result, pattern, match =>
            {
                var value = match.Groups[2].Value;
                return match.Value.Replace(value, MaskSensitiveValue(value));
            }, RegexOptions.Compiled);
        }

        return result;
    }

    public IReadOnlyDictionary<string, string> MaskDictionary(
        IReadOnlyDictionary<string, string> data,
        params string[] sensitiveKeys)
    {
        var masked = new Dictionary<string, string>(data);

        foreach (var key in sensitiveKeys)
        {
            if (masked.ContainsKey(key))
            {
                masked[key] = MaskSensitiveValue(masked[key]);
            }
        }

        return masked;
    }
}
