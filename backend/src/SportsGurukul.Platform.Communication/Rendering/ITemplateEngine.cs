namespace SportsGurukul.Platform.Communication.Rendering;

public interface ITemplateEngine
{
    string Name { get; }
    Task<string> RenderAsync(string template, IReadOnlyDictionary<string, object> variables);
    IReadOnlyList<string> ExtractVariables(string template);
}
