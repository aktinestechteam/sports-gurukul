using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed partial class ContentClassifier : IContentClassifier
{
    public string Name => "KeywordContentClassifier";

    private static readonly IReadOnlyList<(string Category, string[] Keywords)> Rules =
    [
        ("Medical", ["patient", "diagnosis", "treatment", "dosage", "symptom", "clinical", "physiotherapy", "injury", "rehabilitation", "fracture"]),
        ("Financial", ["invoice", "payment", "revenue", "expense", "budget", "salary", "audit", "tax", "balance", "account"]),
        ("Legal", ["agreement", "contract", "clause", "liability", "indemnity", "jurisdiction", "compliance", "regulation", "statute"]),
        ("Sport", ["athlete", "coach", "training", "tournament", "match", "score", "fixture", "player", "tactics", "warmup", "fitness"]),
        ("Education", ["syllabus", "lesson", "curriculum", "exam", "assessment", "student", "module", "workshop", "course"]),
        ("HumanResources", ["resume", "candidate", "interview", "onboarding", "leave", "attendance", "performance review", "appraisal"]),
        ("Marketing", ["campaign", "audience", "brand", "promotion", "social media", "engagement", "sponsorship"]),
        ("General", [])
    ];

    public Task<ContentClassification> ClassifyAsync(string text, CancellationToken ct = default)
    {
        var sample = text.Length <= 8192 ? text : text[..8192];
        var tokens = TokenRegex().Matches(sample.ToLowerInvariant()).Select(m => m.Value).ToHashSet();
        var totalTokens = tokens.Count;

        var bestCategory = "General";
        var bestScore = 0.0;

        foreach (var (category, keywords) in Rules)
        {
            if (keywords.Length == 0)
            {
                continue;
            }

            var hits = keywords.Count(tokens.Contains);
            if (hits == 0)
            {
                continue;
            }

            var score = hits / (double)Math.Min(keywords.Length, 8);
            if (score > bestScore)
            {
                bestScore = score;
                bestCategory = category;
            }
        }

        var matchedTags = Rules
            .Where(r => r.Keywords.Any(tokens.Contains))
            .SelectMany(r => r.Keywords.Where(tokens.Contains))
            .Take(10)
            .ToList();

        var confidence = Math.Clamp(bestScore, 0.0, 1.0);
        if (bestCategory == "General")
        {
            confidence = 0.3;
        }

        return Task.FromResult(new ContentClassification(bestCategory, confidence, matchedTags));
    }

    [GeneratedRegex(@"\b[a-z]{3,}\b")]
    private static partial Regex TokenRegex();
}
