using System.Text.RegularExpressions;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public partial class TemplateRenderer : ITemplateRenderer
{
    private static readonly Regex VariablePattern = VariableRegex();

    [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
    private static partial Regex VariableRegex();

    public Task<Result<(string Subject, string Body)>> RenderAsync(
        string subjectTemplate,
        string bodyTemplate,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        var subject = ReplaceVariables(subjectTemplate, variables);
        var body = ReplaceVariables(bodyTemplate, variables);

        return Task.FromResult(Result<(string, string)>.Success((subject, body)));
    }

    public IReadOnlyList<string> ExtractVariables(string template)
    {
        return VariablePattern.Matches(template)
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();
    }

    private static string ReplaceVariables(string template, IReadOnlyDictionary<string, string> variables)
    {
        return VariablePattern.Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            return variables.TryGetValue(key, out var value) ? value : match.Value;
        });
    }
}
