using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface ITemplateRenderer
{
    Task<Result<(string Subject, string Body)>> RenderAsync(
        string subjectTemplate,
        string bodyTemplate,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default);

    IReadOnlyList<string> ExtractVariables(string template);
}
