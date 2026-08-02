using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.MultiAgent;

public class ResultAggregator : IResultAggregator
{
    private readonly ILogger<ResultAggregator> _logger;

    public ResultAggregator(ILogger<ResultAggregator>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ResultAggregator>.Instance;
    }

    public Task<AggregationResult> AggregateAsync(IReadOnlyList<DelegatedTaskResult> results, AggregationStrategy strategy, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var succeeded = results.Where(r => r.Succeeded).ToList();
        if (succeeded.Count == 0)
        {
            var errors = results
                .Select(r => r.Error)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e!)
                .ToList();
            return Task.FromResult(new AggregationResult
            {
                Succeeded = false,
                Strategy = strategy,
                ResultCount = results.Count,
                Answer = "No worker produced a successful result.",
                Notes = errors.Count > 0 ? errors.Take(5).ToList() : null
            });
        }

        string? answer;
        var notes = new List<string>();

        switch (strategy)
        {
            case AggregationStrategy.FirstSuccess:
                answer = succeeded.First().Answer;
                break;

            case AggregationStrategy.BestScore:
                var best = succeeded.OrderByDescending(r => r.Score ?? 0).First();
                answer = best.Answer;
                notes.Add($"Best score: {best.Score?.ToString("0.##") ?? "n/a"} from {best.AgentId}");
                break;

            case AggregationStrategy.Vote:
                answer = succeeded
                    .Where(r => !string.IsNullOrWhiteSpace(r.Answer))
                    .Select(r => r.Answer!)
                    .GroupBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .ThenByDescending(g => g.Key.Length)
                    .Select(g => g.Key)
                    .FirstOrDefault();
                break;

            case AggregationStrategy.Merge:
                answer = string.Join("; ", succeeded
                    .Where(r => !string.IsNullOrWhiteSpace(r.Answer))
                    .Select(r => r.Answer)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
                break;

            case AggregationStrategy.Concatenate:
                answer = string.Join(Environment.NewLine, succeeded
                    .Where(r => !string.IsNullOrWhiteSpace(r.Answer))
                    .Select(r => $"{r.AgentId}: {r.Answer}"));
                break;

            default:
                answer = succeeded.First().Answer;
                break;
        }

        if (string.IsNullOrWhiteSpace(answer))
        {
            return Task.FromResult(new AggregationResult
            {
                Succeeded = false,
                Strategy = strategy,
                ResultCount = results.Count,
                Answer = "No worker produced a non-empty answer.",
                Notes = notes
            });
        }

        return Task.FromResult(new AggregationResult
        {
            Succeeded = true,
            Answer = answer,
            Strategy = strategy,
            ResultCount = succeeded.Count,
            Notes = notes.Count > 0 ? notes : null
        });
    }
}
