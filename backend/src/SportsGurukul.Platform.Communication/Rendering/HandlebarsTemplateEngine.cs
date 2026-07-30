using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Rendering;

public partial class HandlebarsTemplateEngine : ITemplateEngine
{
    private readonly ILogger<HandlebarsTemplateEngine> _logger;

    public string Name => "Handlebars";

    public HandlebarsTemplateEngine(ILogger<HandlebarsTemplateEngine> logger)
    {
        _logger = logger;
    }

    public Task<string> RenderAsync(string template, IReadOnlyDictionary<string, object> variables)
    {
        var result = RenderConditionals(template, variables);
        result = RenderLoops(result, variables);
        result = RenderVariables(result, variables);
        result = RenderPartial(result, variables);

        return Task.FromResult(result);
    }

    public IReadOnlyList<string> ExtractVariables(string template)
    {
        var matches = SimpleVariableRegex().Matches(template);
        var variables = new HashSet<string>();

        foreach (Match match in matches)
            variables.Add(match.Groups[1].Value);

        foreach (Match match in EachBlockRegex().Matches(template))
            variables.Add(match.Groups[1].Value);

        return variables.ToList();
    }

    private static string RenderVariables(string template, IReadOnlyDictionary<string, object> variables)
    {
        return SimpleVariableRegex().Replace(template, match =>
        {
            var key = match.Groups[1].Value.Trim();
            var parts = key.Split('|');
            var varName = parts[0].Trim();
            var defaultValue = parts.Length > 1 ? parts[1].Trim() : null;

            if (ResolveNestedVariable(varName, variables) is object value && value is not null)
                return value.ToString()!;

            if (defaultValue is not null)
                return defaultValue;

            return match.Value;
        });
    }

    private static string RenderConditionals(string template, IReadOnlyDictionary<string, object> variables)
    {
        template = IfBlockRegex().Replace(template, match =>
        {
            var condition = match.Groups[1].Value.Trim();
            var content = match.Groups[2].Value;
            var negate = condition.StartsWith('!');
            var varName = negate ? condition[1..].Trim() : condition;

            var value = ResolveNestedVariable(varName, variables);
            var isTruthy = value is not null && value.ToString() != "false" && value.ToString() != "";

            if (negate ? !isTruthy : isTruthy)
            {
                var elseMatch = ElseBlockRegex().Match(content);
                return elseMatch.Success ? elseMatch.Groups[1].Value.Trim() : content;
            }

            var elseBlockMatch = ElseBlockRegex().Match(content);
            return elseBlockMatch.Success ? elseBlockMatch.Groups[2].Value.Trim() : string.Empty;
        });

        return template;
    }

    private static string RenderLoops(string template, IReadOnlyDictionary<string, object> variables)
    {
        return EachBlockRegex().Replace(template, match =>
        {
            var collectionName = match.Groups[1].Value.Trim();
            var content = match.Groups[2].Value;

            if (ResolveNestedVariable(collectionName, variables) is not IEnumerable<object> items)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            var index = 0;

            foreach (var item in items)
            {
                var itemContext = new Dictionary<string, object>(variables)
                {
                    ["this"] = item,
                    ["@index"] = index,
                    ["@first"] = index == 0,
                    ["@last"] = index == items.Count() - 1
                };

                if (item is Dictionary<string, object> dict)
                {
                    foreach (var kv in dict)
                        itemContext[kv.Key] = kv.Value;
                }

                sb.Append(RenderVariables(content, itemContext));
                index++;
            }

            return sb.ToString();
        });
    }

    private static string RenderPartial(string template, IReadOnlyDictionary<string, object> variables)
    {
        return PartialRegex().Replace(template, match =>
        {
            return string.Empty;
        });
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

    [GeneratedRegex(@"\{\{(.+?)\}\}", RegexOptions.Compiled)]
    private static partial Regex SimpleVariableRegex();

    [GeneratedRegex(@"\{\{#if\s+(.+?)\}\}(.*?)\{\{/if\}\}", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex IfBlockRegex();

    [GeneratedRegex(@"\{\{#each\s+(.+?)\}\}(.*?)\{\{/each\}\}", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex EachBlockRegex();

    [GeneratedRegex(@"\{\{else\}\}(.*?)(?=\{\{/if\}\})", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex ElseBlockRegex();

    [GeneratedRegex(@"\{\{>\s*(.+?)\}\}", RegexOptions.Compiled)]
    private static partial Regex PartialRegex();
}
