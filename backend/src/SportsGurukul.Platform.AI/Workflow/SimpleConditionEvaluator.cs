using System.Globalization;
using SportsGurukul.Platform.AI.Interfaces.Workflow;

namespace SportsGurukul.Platform.AI.Workflow;

public class SimpleConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(string? condition, IReadOnlyDictionary<string, object?> state)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return true;
        }

        var token = condition.Trim();

        if (token.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (token.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (token.StartsWith("exists ", StringComparison.OrdinalIgnoreCase))
        {
            var key = token["exists ".Length..].Trim();
            return state.TryGetValue(key, out var value) && value is not null;
        }

        var operators = new[] { ">=", "<=", "!=", "==", ">", "<", " contains ", " in " };
        foreach (var op in operators)
        {
            var index = token.IndexOf(op, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                continue;
            }

            var left = token[..index].Trim();
            var right = token[(index + op.Length)..].Trim();
            return Compare(left, op.Trim(), right, state);
        }

        return false;
    }

    private static bool Compare(string left, string op, string right, IReadOnlyDictionary<string, object?> state)
    {
        var leftValue = Resolve(left, state);
        var rightValue = Unquote(right);

        switch (op)
        {
            case "==":
                return ValuesEqual(leftValue, rightValue);
            case "!=":
                return !ValuesEqual(leftValue, rightValue);
            case ">":
            case "<":
            case ">=":
            case "<=":
                return CompareNumbers(leftValue, rightValue, op);
            case "contains":
                return leftValue?.ToString()?.Contains(rightValue, StringComparison.OrdinalIgnoreCase) == true;
            case "in":
                return rightValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(item => ValuesEqual(leftValue, item));
            default:
                return false;
        }
    }

    private static object? Resolve(string key, IReadOnlyDictionary<string, object?> state) =>
        state.TryGetValue(key, out var value) ? value : key;

    private static string Unquote(string value)
    {
        if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
        {
            return value[1..^1];
        }

        return value;
    }

    private static bool ValuesEqual(object? left, string right)
    {
        if (left is null)
        {
            return string.Equals(right, "null", StringComparison.OrdinalIgnoreCase);
        }

        if (left is bool b)
        {
            return bool.TryParse(right, out var rb) && b == rb;
        }

        return string.Equals(left.ToString(), right, StringComparison.OrdinalIgnoreCase);
    }

    private static bool CompareNumbers(object? left, string right, string op)
    {
        if (left is null || !double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out var rightNumber))
        {
            return false;
        }

        if (!double.TryParse(left.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var leftNumber))
        {
            return false;
        }

        return op switch
        {
            ">" => leftNumber > rightNumber,
            "<" => leftNumber < rightNumber,
            ">=" => leftNumber >= rightNumber,
            "<=" => leftNumber <= rightNumber,
            _ => false
        };
    }
}
