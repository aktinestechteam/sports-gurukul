using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Rendering;

public partial class LiquidTemplateEngine : ITemplateEngine
{
    private readonly ILogger<LiquidTemplateEngine> _logger;

    public string Name => "Liquid";

    public LiquidTemplateEngine(ILogger<LiquidTemplateEngine> logger)
    {
        _logger = logger;
    }

    public Task<string> RenderAsync(string template, IReadOnlyDictionary<string, object> variables)
    {
        var result = RenderFilters(template, variables);
        result = RenderConditionals(result, variables);
        result = RenderLoops(result, variables);
        result = RenderVariables(result, variables);

        return Task.FromResult(result);
    }

    public IReadOnlyList<string> ExtractVariables(string template)
    {
        var matches = VariableRegex().Matches(template);
        var variables = new HashSet<string>();

        foreach (Match match in matches)
        {
            var content = match.Groups[1].Value.Trim();
            var pipeIdx = content.IndexOf('|');
            var varName = pipeIdx >= 0 ? content[..pipeIdx].Trim() : content.Trim();
            variables.Add(varName);
        }

        return variables.ToList();
    }

    private static string RenderVariables(string template, IReadOnlyDictionary<string, object> variables)
    {
        return VariableRegex().Replace(template, match =>
        {
            var expression = match.Groups[1].Value.Trim();
            var parts = expression.Split('|');
            var varPath = parts[0].Trim();

            object? value = ResolveNestedVariable(varPath, variables);

            for (var i = 1; i < parts.Length; i++)
            {
                var filter = parts[i].Trim();
                value = ApplyFilter(filter, value);
            }

            return value?.ToString() ?? match.Value;
        });
    }

    private static string RenderConditionals(string template, IReadOnlyDictionary<string, object> variables)
    {
        template = IfRegex().Replace(template, match =>
        {
            var condition = match.Groups[1].Value.Trim();
            var content = match.Groups[2].Value;

            var elseSplit = ElseRegex().Match(content);
            var trueContent = elseSplit.Success ? elseSplit.Groups[1].Value.Trim() : content;
            var falseContent = elseSplit.Success ? elseSplit.Groups[2].Value.Trim() : string.Empty;

            var conditionResult = EvaluateCondition(condition, variables);
            return conditionResult ? trueContent : falseContent;
        });

        return template;
    }

    private static string RenderLoops(string template, IReadOnlyDictionary<string, object> variables)
    {
        return ForRegex().Replace(template, match =>
        {
            var itemName = match.Groups[1].Value.Trim();
            var collectionPath = match.Groups[2].Value.Trim();
            var content = match.Groups[3].Value;

            if (ResolveNestedVariable(collectionPath, variables) is not IEnumerable<object> items)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            var idx = 0;

            foreach (var item in items)
            {
                var context = new Dictionary<string, object>(variables)
                {
                    [itemName] = item,
                    ["forloop"] = new Dictionary<string, object>
                    {
                        ["index"] = idx,
                        ["first"] = idx == 0,
                        ["last"] = idx == items.Count() - 1
                    }
                };

                sb.Append(RenderVariables(content, context));
                idx++;
            }

            return sb.ToString();
        });
    }

    private static string RenderFilters(string template, IReadOnlyDictionary<string, object> variables)
    {
        return template;
    }

    private static object? ResolveNestedVariable(string path, IReadOnlyDictionary<string, object> variables)
    {
        var parts = path.Split('.');
        object? current = null;

        if (variables.TryGetValue(parts[0], out var value))
            current = value;
        else
            return null;

        for (var i = 1; i < parts.Length; i++)
        {
            if (current is Dictionary<string, object> dict && dict.TryGetValue(parts[i], out var nested))
                current = nested;
            else if (current is not null)
            {
                var prop = current.GetType().GetProperty(parts[i]);
                if (prop is not null)
                    current = prop.GetValue(current);
                else
                    return null;
            }
            else
                return null;
        }

        return current;
    }

    private static bool EvaluateCondition(string condition, IReadOnlyDictionary<string, object> variables)
    {
        var parts = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 1)
        {
            var value = ResolveNestedVariable(parts[0], variables);
            return value is not null && value.ToString() != "false" && value.ToString() != "";
        }

        if (parts.Length >= 3)
        {
            var leftValue = parts[0];
            var op = parts[1];
            var rightValue = string.Join(" ", parts.Skip(2));

            var left = ResolveNestedVariable(leftValue, variables)?.ToString() ?? leftValue.Trim('\'', '"');
            var right = rightValue.Trim('\'', '"');

            return op switch
            {
                "==" => string.Equals(left, right, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(left, right, StringComparison.OrdinalIgnoreCase),
                ">" => decimal.TryParse(left, out var ld) && decimal.TryParse(right, out var rd) && ld > rd,
                "<" => decimal.TryParse(left, out var ld) && decimal.TryParse(right, out var rd) && ld < rd,
                ">=" => decimal.TryParse(left, out var ld) && decimal.TryParse(right, out var rd) && ld >= rd,
                "<=" => decimal.TryParse(left, out var ld) && decimal.TryParse(right, out var rd) && ld <= rd,
                "contains" => left?.Contains(right, StringComparison.OrdinalIgnoreCase) ?? false,
                _ => false
            };
        }

        return false;
    }

    private static object? ApplyFilter(string filter, object? value)
    {
        var parts = filter.Split(':', StringSplitOptions.TrimEntries);
        var filterName = parts[0].ToLowerInvariant();
        var arg = parts.Length > 1 ? parts[1].Trim() : null;

        return filterName switch
        {
            "upcase" => value?.ToString()?.ToUpperInvariant(),
            "downcase" => value?.ToString()?.ToLowerInvariant(),
            "capitalize" => value?.ToString() switch
            {
                null => null,
                string s => char.ToUpperInvariant(s[0]) + s[1..]
            },
            "strip" => value?.ToString()?.Trim(),
            "escape" => System.Net.WebUtility.HtmlEncode(value?.ToString() ?? ""),
            "default" => value ?? arg,
            "truncate" when arg is not null && int.TryParse(arg, out var len)
                => value?.ToString()?.Length > len ? value.ToString()![..len] + "..." : value?.ToString(),
            _ => value
        };
    }

    [GeneratedRegex(@"\{\{(.+?)\}\}", RegexOptions.Compiled)]
    private static partial Regex VariableRegex();

    [GeneratedRegex(@"\{%\s*if\s+(.+?)\s*%\}(.*?)\{%\s*endif\s*%\}", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex IfRegex();

    [GeneratedRegex(@"\{%\s*else\s*%\}(.*?)(?=\{%\s*endif\s*%\})", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex ElseRegex();

    [GeneratedRegex(@"\{%\s*for\s+(.+?)\s+in\s+(.+?)\s*%\}(.*?)\{%\s*endfor\s*%\}", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex ForRegex();
}
