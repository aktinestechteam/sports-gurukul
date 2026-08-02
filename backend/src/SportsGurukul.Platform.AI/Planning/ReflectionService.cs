using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Interfaces.Planning;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Planning;

public class ReflectionService : IReflectionService
{
    private readonly ILanguageModel? _model;
    private readonly ILogger<ReflectionService> _logger;

    public ReflectionService(ILanguageModel? model = null, ILogger<ReflectionService>? logger = null)
    {
        _model = model;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ReflectionService>.Instance;
    }

    public async Task<Reflection> ReflectAsync(ReflectionRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_model is not null)
        {
            var prompt = BuildReflectionPrompt(request);
            var response = await _model.GenerateAsync([ModelMessage.User(prompt)], cancellationToken: cancellationToken);
            return ParseReflection(response.Content, request.PlanId);
        }

        var completed = request.CompletedSteps ?? [];
        var failures = completed.Count(r => !r.Succeeded);
        var successes = completed.Count - failures;
        var score = completed.Count == 0 ? 0.5 : successes / (double)completed.Count;

        var shouldReplan = failures > 0 && !string.IsNullOrWhiteSpace(request.Insight);
        return new Reflection
        {
            PlanId = request.PlanId,
            Score = score,
            Insight = request.Insight ?? (shouldReplan ? "Detected step failures; plan requires adjustment." : "Plan progressing as expected."),
            Improvement = shouldReplan ? "Reprioritize failed steps and add recovery actions." : null,
            ShouldReplan = shouldReplan,
            ShouldStop = score == 0
        };
    }

    public async Task<SelfEvaluation> EvaluateAsync(SelfEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_model is not null)
        {
            var prompt = BuildEvaluationPrompt(request);
            var response = await _model.GenerateAsync([ModelMessage.User(prompt)], cancellationToken: cancellationToken);
            return ParseEvaluation(response.Content, request.RunId);
        }

        var tasks = request.Tasks ?? [];
        if (tasks.Count == 0)
        {
            return new SelfEvaluation { RunId = request.RunId, Score = 0.5, Verdict = "Partial" };
        }

        var succeeded = tasks.Count(t => t.Succeeded);
        var score = succeeded / (double)tasks.Count;

        return new SelfEvaluation
        {
            RunId = request.RunId,
            Score = score,
            Verdict = score == 1 ? "Success" : score >= 0.5 ? "Partial" : "Failed",
            Strengths = score >= 0.5 ? ["Completed majority of planned tasks"] : [],
            Weaknesses = score < 1 ? ["Some planned tasks did not complete successfully"] : [],
            Improvements = score < 1 ? ["Retry failed tasks and refine plan"] : []
        };
    }

    private static string BuildReflectionPrompt(ReflectionRequest request)
    {
        var completed = request.CompletedSteps ?? [];
        var lines = string.Join("\n", completed.Select(c => $"- {(c.Succeeded ? "OK" : "FAIL")}: {c.Step.Title}"));
        return $"Reflect on the following agent run.\nGoal: {request.Goal}\nCompleted steps:\n{lines}\nInsight: {request.Insight}\nReturn JSON: {{ \"score\": 0.0-1.0, \"insight\": \"...\", \"improvement\": \"...\", \"shouldReplan\": bool, \"shouldStop\": bool }}";
    }

    private static string BuildEvaluationPrompt(SelfEvaluationRequest request)
    {
        var tasks = request.Tasks ?? [];
        var lines = string.Join("\n", tasks.Select(c => $"- {(c.Succeeded ? "OK" : "FAIL")}: {c.Step.Title}"));
        return $"Self-evaluate this agent run.\nGoal: {request.Goal}\nFinal answer: {request.FinalAnswer}\nTasks:\n{lines}\nReturn JSON: {{ \"score\": 0.0-1.0, \"verdict\": \"...\", \"strengths\": [...], \"weaknesses\": [...], \"improvements\": [...] }}";
    }

    private static Reflection ParseReflection(string content, string? planId)
    {
        var score = ExtractDouble(content, "score") ?? 0.5;
        var shouldReplan = content.Contains("\"shouldReplan\"", StringComparison.OrdinalIgnoreCase) &&
                           !content.Contains("\"shouldReplan\": false", StringComparison.OrdinalIgnoreCase);
        var shouldStop = content.Contains("\"shouldStop\"", StringComparison.OrdinalIgnoreCase) &&
                         content.Contains("\"shouldStop\": true", StringComparison.OrdinalIgnoreCase);

        return new Reflection
        {
            PlanId = planId,
            Score = Math.Clamp(score, 0, 1),
            Insight = ExtractString(content, "insight") ?? "Reflection completed.",
            Improvement = ExtractString(content, "improvement"),
            ShouldReplan = shouldReplan,
            ShouldStop = shouldStop
        };
    }

    private static SelfEvaluation ParseEvaluation(string content, string? runId)
    {
        var score = ExtractDouble(content, "score") ?? 0.5;
        return new SelfEvaluation
        {
            RunId = runId,
            Score = Math.Clamp(score, 0, 1),
            Verdict = ExtractString(content, "verdict") ?? (score >= 0.5 ? "Partial" : "Failed"),
            Strengths = ExtractArray(content, "strengths"),
            Weaknesses = ExtractArray(content, "weaknesses"),
            Improvements = ExtractArray(content, "improvements")
        };
    }

    private static double? ExtractDouble(string json, string key)
    {
        var marker = $"\"{key}\"";
        var index = json.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        var valuePart = json[(index + marker.Length)..];
        var colon = valuePart.IndexOf(':');
        if (colon < 0)
        {
            return null;
        }

        valuePart = valuePart[(colon + 1)..].TrimStart();
        var token = new string(valuePart.TakeWhile(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
        return double.TryParse(token, System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : null;
    }

    private static string? ExtractString(string json, string key)
    {
        var marker = $"\"{key}\"";
        var index = json.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        var valuePart = json[(index + marker.Length)..];
        var colon = valuePart.IndexOf(':');
        if (colon < 0)
        {
            return null;
        }

        valuePart = valuePart[(colon + 1)..].TrimStart();
        if (!valuePart.StartsWith('"'))
        {
            return null;
        }

        valuePart = valuePart[1..];
        var end = valuePart.IndexOf('"');
        return end < 0 ? null : valuePart[..end];
    }

    private static IReadOnlyList<string> ExtractArray(string json, string key)
    {
        var marker = $"\"{key}\"";
        var index = json.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return [];
        }

        var valuePart = json[(index + marker.Length)..];
        var colon = valuePart.IndexOf(':');
        if (colon < 0)
        {
            return [];
        }

        valuePart = valuePart[(colon + 1)..].TrimStart();
        if (!valuePart.StartsWith('['))
        {
            return [];
        }

        var results = new List<string>();
        var remaining = valuePart[1..];
        var quote = remaining.IndexOf('"');
        while (quote >= 0)
        {
            var end = remaining.IndexOf('"', quote + 1);
            if (end < 0)
            {
                break;
            }

            results.Add(remaining[(quote + 1)..end]);
            remaining = remaining[(end + 1)..];
            quote = remaining.IndexOf('"');
        }

        return results;
    }
}
