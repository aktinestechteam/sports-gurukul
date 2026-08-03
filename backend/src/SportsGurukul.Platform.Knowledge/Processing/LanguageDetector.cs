using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed partial class LanguageDetector : ILanguageDetector
{
    private static readonly Dictionary<string, HashSet<char>> ScriptSets = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hi"] = BuildRange('\u0900', '\u097F'),
        ["bn"] = BuildRange('\u0980', '\u09FF'),
        ["ta"] = BuildRange('\u0B80', '\u0BFF'),
        ["te"] = BuildRange('\u0C00', '\u0C7F'),
        ["ml"] = BuildRange('\u0D00', '\u0D7F'),
        ["kn"] = BuildRange('\u0C80', '\u0CFF'),
        ["gu"] = BuildRange('\u0A80', '\u0AFF'),
        ["pa"] = BuildRange('\u0A00', '\u0A7F'),
        ["ar"] = BuildRange('\u0600', '\u06FF'),
        ["zh"] = BuildRange('\u4E00', '\u9FFF'),
        ["ja"] = BuildRange('\u3040', '\u30FF'),
        ["ko"] = BuildRange('\uAC00', '\uD7AF'),
        ["el"] = BuildRange('\u0370', '\u03FF'),
        ["ru"] = BuildRange('\u0400', '\u04FF')
    };

    private static readonly Dictionary<string, string[]> StopWords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = ["the", "and", "for", "with", "this", "that", "are", "was", "from", "have", "not", "you"],
            ["es"] = ["el", "la", "los", "las", "de", "del", "y", "en", "un", "una", "es", "por"],
            ["fr"] = ["le", "la", "les", "de", "des", "du", "et", "en", "un", "une", "est", "pour"],
            ["de"] = ["der", "die", "das", "und", "in", "den", "von", "mit", "ein", "eine", "ist", "für"],
            ["pt"] = ["o", "a", "os", "as", "de", "do", "da", "em", "um", "uma", "é", "para"],
            ["it"] = ["il", "lo", "la", "gli", "le", "di", "del", "e", "in", "un", "una", "per"],
            ["hi"] = ["और", "है", "का", "की", "के", "में", "यह", "वह", "से", "पर", "को", "एक"]
        };

    private readonly CultureInfo _defaultCulture;

    public LanguageDetector(string defaultLanguage = "en")
    {
        _defaultCulture = CultureInfo.GetCultureInfo(defaultLanguage);
    }

    public string Detect(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return _defaultCulture.TwoLetterISOLanguageName;
        }

        var scriptScores = CountScripts(text);
        var totalScriptChars = scriptScores.Values.Sum();
        if (totalScriptChars > 0)
        {
            var dominant = scriptScores.OrderByDescending(kv => kv.Value).First();
            var ratio = dominant.Value / (double)totalScriptChars;
            if (ratio >= 0.2)
            {
                return dominant.Key;
            }
        }

        return DetectByStopWords(text);
    }

    private string DetectByStopWords(string text)
    {
        var tokens = TokenRegex().Matches(text.ToLowerInvariant())
            .Select(m => m.Value)
            .Where(t => t.Length >= 2)
            .ToList();
        if (tokens.Count == 0)
        {
            return _defaultCulture.TwoLetterISOLanguageName;
        }

        var tokenSet = new HashSet<string>(tokens);
        var bestLanguage = _defaultCulture.TwoLetterISOLanguageName;
        var bestScore = 0.0;
        var totalWeight = 0;

        foreach (var (language, words) in StopWords)
        {
            var hits = words.Count(w => tokenSet.Contains(w));
            var weight = words.Length;
            var score = hits / (double)Math.Max(1, weight);
            if (score > bestScore)
            {
                bestScore = score;
                bestLanguage = language;
            }

            totalWeight += weight;
        }

        return bestScore <= 0 ? _defaultCulture.TwoLetterISOLanguageName : bestLanguage;
    }

    private static IReadOnlyDictionary<string, int> CountScripts(string text)
    {
        var scores = new Dictionary<string, int>();
        foreach (var c in text)
        {
            foreach (var (script, set) in ScriptSets)
            {
                if (set.Contains(c))
                {
                    scores[script] = scores.TryGetValue(script, out var value) ? value + 1 : 1;
                    break;
                }
            }
        }

        return scores;
    }

    private static HashSet<char> BuildRange(int start, int end)
    {
        var set = new HashSet<char>();
        for (var i = start; i <= end; i++)
        {
            set.Add((char)i);
        }

        return set;
    }

    [GeneratedRegex(@"\b[a-z\u0900-\u097F]+\b", RegexOptions.Multiline)]
    private static partial Regex TokenRegex();
}
